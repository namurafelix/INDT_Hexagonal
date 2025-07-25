using ContratacaoService.Application.Services;
using System.Net.Http;
using System.Threading.Tasks;
using System;
using System.Text.Json;

namespace ContratacaoService.Infrastructure.Clients;

public class PropostaServiceClient : IPropostaServiceClient
{
    private readonly HttpClient _httpClient;

    public PropostaServiceClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<string> ObterStatusPropostaAsync(Guid propostaId)
    {
        var response = await _httpClient.GetAsync($"/api/propostas/{propostaId}/status");
        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadAsStringAsync();
        
        // Assuming the API returns a simple string status or a JSON object with a "status" field
        try
        {
            using (JsonDocument doc = JsonDocument.Parse(content))
            {
                if (doc.RootElement.TryGetProperty("status", out JsonElement statusElement))
                {
                    return statusElement.GetString() ?? "Unknown";
                }
            }
        }
        catch (JsonException)
        {
            // If it's not JSON, assume it's a plain string status
            return content;
        }
        return "Unknown";
    }
}
