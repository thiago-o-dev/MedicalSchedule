using System.Net.Http.Headers;
using System.Text.Json;
using Api.Gateway.Models;
using Microsoft.AspNetCore.Mvc;

namespace Api.Gateway.Controllers;

[ApiController]
[Route("api/auth")]
public sealed class AuthController(IHttpClientFactory httpClientFactory, IConfiguration configuration) : ControllerBase
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

        var form = new FormUrlEncodedContent(
        [
            new KeyValuePair<string, string>("grant_type", "password"),
            new KeyValuePair<string, string>("client_id", ClientId),
            new KeyValuePair<string, string>("username", request.Email),
            new KeyValuePair<string, string>("password", request.Password),
        ]);

        var response = await client.PostAsync(
            $"{KeycloakBase}/realms/{Realm}/protocol/openid-connect/token", form, ct);

        var body = await response.Content.ReadAsStringAsync(ct);

        if (!response.IsSuccessStatusCode)
            return StatusCode((int)response.StatusCode, body);

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

        var parts = request.Name.Trim().Split(' ', 2);

        var createResp = await client.PostAsJsonAsync(
            $"{KeycloakBase}/admin/realms/{Realm}/users",
            new
            {
                username = request.Email,
                email = request.Email,
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

        if (request.IsOwner)
        {
            await client.PostAsJsonAsync($"{RegistryBase}/api/owners", new
            {
                name = request.Name,
                cpf = request.Document,
                phone = request.Phone,
                email = request.Email
            }, ct);
        }
        else
        {
            await client.PostAsJsonAsync($"{RegistryBase}/api/vets", new
            {
                name = request.Name,
                crm = request.Document,
                specialty = request.Specialty ?? "General",
                email = request.Email
            }, ct);
        }

        var form = new FormUrlEncodedContent(
        [
            new KeyValuePair<string, string>("grant_type", "password"),
            new KeyValuePair<string, string>("client_id", ClientId),
            new KeyValuePair<string, string>("username", request.Email),
            new KeyValuePair<string, string>("password", request.Password),
        ]);

        var tokenResp = await client.PostAsync(
            $"{KeycloakBase}/realms/{Realm}/protocol/openid-connect/token", form, ct);

        var tokenBody = await tokenResp.Content.ReadAsStringAsync(ct);
        if (!tokenResp.IsSuccessStatusCode)
            return StatusCode((int)tokenResp.StatusCode, tokenBody);

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
}
