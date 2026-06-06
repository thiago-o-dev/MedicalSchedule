using System.Net.Http.Json;

namespace Notifications.Infrastructure.External;

public sealed class RegistryClient(HttpClient httpClient) : IRegistryClient
{
    public async Task<OwnerContact?> GetOwnerByPetIdAsync(Guid petId, CancellationToken cancellationToken = default)
    {
        var response = await httpClient.GetAsync($"api/pets/{petId}/owner", cancellationToken);

        if (!response.IsSuccessStatusCode)
            return null;

        return await response.Content.ReadFromJsonAsync<OwnerContact>(cancellationToken);
    }
}
