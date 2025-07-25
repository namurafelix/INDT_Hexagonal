using Microsoft.AspNetCore.Mvc.Testing;
using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Xunit;
using Microsoft.AspNetCore.Hosting;

namespace PropostaService.IntegrationTests;

public class PropostaEndpointsTests : IClassFixture<WebApplicationFactory<PropostaService.Api.Controllers.PropostasController>>
{
    private readonly HttpClient _client;

    public PropostaEndpointsTests(WebApplicationFactory<PropostaService.Api.Controllers.PropostasController> factory)
    {
        _client = factory.WithWebHostBuilder(builder =>
        {
            builder.UseEnvironment("Development");
        }).CreateClient();
    }

    [Fact]
    public async Task Deve_Adicionar_E_Listar_Propostas()
    {
        var proposta = new
        {
            NomeCliente = "João da Silva",
            CPF = "12345678901",
            Idade = 35,
            ValorSeguro = 5000m
        };

        var content = new StringContent(JsonConvert.SerializeObject(proposta), Encoding.UTF8, "application/json");

        var responsePost = await _client.PostAsync("/api/propostas", content);
        responsePost.EnsureSuccessStatusCode();

        var responseGet = await _client.GetAsync("/api/propostas");
        var body = await responseGet.Content.ReadAsStringAsync();

        Assert.Contains("João da Silva", body);
    }
}
