using Microsoft.AspNetCore.Mvc.Testing;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;
using Microsoft.AspNetCore.Hosting;

namespace ContratacaoService.IntegrationTests;

public class ContratacaoEndpointsTests : IClassFixture<WebApplicationFactory<ContratacaoService.Api.Program>>
{
    private readonly HttpClient _client;

    public ContratacaoEndpointsTests(WebApplicationFactory<ContratacaoService.Api.Program> factory)
    {
        _client = factory.WithWebHostBuilder(builder =>
        {
            builder.UseEnvironment("Development");
        }).CreateClient();
    }

    [Fact]
    public async Task Deve_Contratar_Proposta_Aprovada()
    {
        var propostaId = Guid.NewGuid();

        // This test assumes that a proposal with this ID exists and is approved in the PropostaService database.
        // In a real-world scenario, you would first create and approve a proposal using the PropostaService API.
        var response = await _client.PostAsync($"/api/contratacoes?propostaId={propostaId}", null);

        response.EnsureSuccessStatusCode();
    }
}
