using System.Net.Http.Headers;
using System.Text.Json;
using Api.Gateway.Models;
using Microsoft.AspNetCore.Mvc;

namespace Api.Gateway.Controllers;

[ApiController]
[Route("api/auth")]
public sealed class AuthController(
    IHttpClientFactory httpClientFactory,
    IConfiguration configuration,
    ILogger<AuthController> logger) : ControllerBase
{
    private string KeycloakBase => configuration["Keycloak:BaseUrl"]!;
    private string Realm => configuration["Keycloak:Realm"]!;
    private string ClientId => configuration["Keycloak:ClientId"]!;
    private string AdminUser => configuration["Keycloak:AdminUser"]!;
    private string AdminPassword => configuration["Keycloak:AdminPassword"]!;
    private string RegistryBase => configuration["Registry:BaseUrl"]!;

    [HttpPost("token")]
    public async Task<IActionResult> Token([FromBody] TokenRequest request, CancellationToken ct)
    {
        using var client = httpClientFactory.CreateClient("keycloak");

        var username = request.Email.Trim().ToLowerInvariant();

        var form = new FormUrlEncodedContent(
        [
            new KeyValuePair<string, string>("grant_type", "password"),
            new KeyValuePair<string, string>("client_id", ClientId),
            new KeyValuePair<string, string>("username", username),
            new KeyValuePair<string, string>("password", request.Password),
        ]);

        var response = await client.PostAsync(
            $"{KeycloakBase}/realms/{Realm}/protocol/openid-connect/token", form, ct);

        var body = await response.Content.ReadAsStringAsync(ct);

        if (!response.IsSuccessStatusCode)
        {
            logger.LogWarning(
                "Keycloak token request failed. Username={Username} Status={Status} Body={Body}",
                username, (int)response.StatusCode, body);
            return new ContentResult
            {
                StatusCode = (int)response.StatusCode,
                Content = body,
                ContentType = "application/json"
            };
        }

        using var json = JsonDocument.Parse(body);
        var token = json.RootElement.GetProperty("access_token").GetString();
        var expiresIn = json.RootElement.GetProperty("expires_in").GetInt32();

        return Ok(new { token, expiresIn });
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request, CancellationToken ct)
    {
        using var client = httpClientFactory.CreateClient("keycloak");

        var adminToken = await GetAdminTokenAsync(client, ct);
        if (adminToken is null)
            return StatusCode(502, "Unable to authenticate with Keycloak admin.");

        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", adminToken);

        var email = request.Email.Trim().ToLowerInvariant();
        var parts = request.Name.Trim().Split(' ', 2);

        var createResp = await client.PostAsJsonAsync(
            $"{KeycloakBase}/admin/realms/{Realm}/users",
            new
            {
                username = email,
                email = email,
                firstName = parts[0],
                lastName = parts.Length > 1 ? parts[1] : parts[0],
                enabled = true,
                emailVerified = true,
                requiredActions = Array.Empty<string>(),
            },
            ct);

        if (!createResp.IsSuccessStatusCode)
            return StatusCode((int)createResp.StatusCode,
                await createResp.Content.ReadAsStringAsync(ct));

        var userId = createResp.Headers.Location!.Segments.Last().TrimEnd('/');

        var pwResp = await client.PutAsJsonAsync(
            $"{KeycloakBase}/admin/realms/{Realm}/users/{userId}/reset-password",
            new { type = "password", value = request.Password, temporary = false },
            ct);

        if (!pwResp.IsSuccessStatusCode)
            return StatusCode((int)pwResp.StatusCode,
                await pwResp.Content.ReadAsStringAsync(ct));

        await AssignRoleAsync(client, userId, request.IsOwner ? "owner" : "vet", ct);

        client.DefaultRequestHeaders.Authorization = null;

        HttpResponseMessage registryResp;
        if (request.IsOwner)
        {
            registryResp = await client.PostAsJsonAsync($"{RegistryBase}/api/owners", new
            {
                name = request.Name,
                cpf = request.Document,
                phone = request.Phone,
                email = email
            }, ct);
        }
        else
        {
            registryResp = await client.PostAsJsonAsync($"{RegistryBase}/api/vets", new
            {
                name = request.Name,
                crm = request.Document,
                specialty = request.Specialty ?? "General",
                email = email
            }, ct);
        }

        if (!registryResp.IsSuccessStatusCode)
        {
            var registryBody = await registryResp.Content.ReadAsStringAsync(ct);
            logger.LogWarning(
                "Registry registration failed. Username={Username} Status={Status} Body={Body}. " +
                "Rolling back Keycloak user {UserId}.",
                email, (int)registryResp.StatusCode, registryBody, userId);

            await DeleteKeycloakUserAsync(client, userId, ct);

            return new ContentResult
            {
                StatusCode = (int)registryResp.StatusCode,
                Content = registryBody,
                ContentType = "application/problem+json"
            };
        }

        var form = new FormUrlEncodedContent(
        [
            new KeyValuePair<string, string>("grant_type", "password"),
            new KeyValuePair<string, string>("client_id", ClientId),
            new KeyValuePair<string, string>("username", email),
            new KeyValuePair<string, string>("password", request.Password),
        ]);

        var tokenResp = await client.PostAsync(
            $"{KeycloakBase}/realms/{Realm}/protocol/openid-connect/token", form, ct);

        var tokenBody = await tokenResp.Content.ReadAsStringAsync(ct);
        if (!tokenResp.IsSuccessStatusCode)
        {
            logger.LogWarning(
                "Keycloak post-register token request failed. Username={Username} Status={Status} Body={Body}",
                email, (int)tokenResp.StatusCode, tokenBody);
            return new ContentResult
            {
                StatusCode = (int)tokenResp.StatusCode,
                Content = tokenBody,
                ContentType = "application/json"
            };
        }

        using var json = JsonDocument.Parse(tokenBody);
        var token = json.RootElement.GetProperty("access_token").GetString();
        var expiresIn = json.RootElement.GetProperty("expires_in").GetInt32();

        return Created(string.Empty, new { token, expiresIn });
    }

    private async Task<string?> GetAdminTokenAsync(HttpClient client, CancellationToken ct)
    {
        var form = new FormUrlEncodedContent(
        [
            new KeyValuePair<string, string>("grant_type", "password"),
            new KeyValuePair<string, string>("client_id", "admin-cli"),
            new KeyValuePair<string, string>("username", AdminUser),
            new KeyValuePair<string, string>("password", AdminPassword),
        ]);

        var response = await client.PostAsync(
            $"{KeycloakBase}/realms/master/protocol/openid-connect/token", form, ct);

        if (!response.IsSuccessStatusCode) return null;

        using var json = JsonDocument.Parse(await response.Content.ReadAsStringAsync(ct));
        return json.RootElement.GetProperty("access_token").GetString();
    }

    private async Task AssignRoleAsync(HttpClient client, string userId, string roleName, CancellationToken ct)
    {
        var roleResp = await client.GetAsync(
            $"{KeycloakBase}/admin/realms/{Realm}/roles/{roleName}", ct);

        if (!roleResp.IsSuccessStatusCode) return;

        var role = await roleResp.Content.ReadAsStringAsync(ct);

        using var content = new StringContent(
            $"[{role}]",
            System.Text.Encoding.UTF8,
            "application/json");

        await client.PostAsync(
            $"{KeycloakBase}/admin/realms/{Realm}/users/{userId}/role-mappings/realm",
            content, ct);
    }

    private async Task DeleteKeycloakUserAsync(HttpClient client, string userId, CancellationToken ct)
    {
        try
        {
            var adminToken = await GetAdminTokenAsync(client, ct);
            if (adminToken is null) return;

            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", adminToken);

            await client.DeleteAsync(
                $"{KeycloakBase}/admin/realms/{Realm}/users/{userId}", ct);
        }
        catch (Exception ex)
        {
            logger.LogError(ex,
                "Failed to roll back Keycloak user {UserId} after Registry failure. " +
                "Manual cleanup may be required.", userId);
        }
        finally
        {
            client.DefaultRequestHeaders.Authorization = null;
        }
    }
}
