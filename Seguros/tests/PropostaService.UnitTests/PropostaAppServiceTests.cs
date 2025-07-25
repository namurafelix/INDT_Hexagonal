using Moq;
using PropostaService.Application.DTOs;
using PropostaService.Application.Services;
using PropostaService.Domain.Entities;
using PropostaService.Domain.Enums;
using PropostaService.Domain.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace PropostaService.UnitTests;

public class PropostaAppServiceTests
{
    private readonly Mock<IPropostaRepository> _propostaRepositoryMock;
    private readonly PropostaAppService _sut;

    public PropostaAppServiceTests()
    {
        _propostaRepositoryMock = new Mock<IPropostaRepository>();
        _sut = new PropostaAppService(_propostaRepositoryMock.Object);
    }

    [Fact]
    public async Task CriarPropostaAsync_Deve_AdicionarProposta_Quando_InputValido()
    {
        // Arrange
        var inputModel = new PropostaInputModel("Cliente Teste", "12345678900", 30, 5000m);
        _propostaRepositoryMock.Setup(r => r.AdicionarAsync(It.IsAny<Proposta>())).Returns(Task.CompletedTask);

        // Act
        var result = await _sut.CriarPropostaAsync(inputModel);

        // Assert
        _propostaRepositoryMock.Verify(r => r.AdicionarAsync(It.Is<Proposta>(p => p.NomeCliente == inputModel.NomeCliente)), Times.Once);
        Assert.NotNull(result);
        Assert.Equal(inputModel.NomeCliente, result.NomeCliente);
    }

    [Fact]
    public async Task ObterPropostaPorIdAsync_Deve_RetornarProposta_Quando_PropostaExiste()
    {
        // Arrange
        var propostaId = Guid.NewGuid();
        var proposta = new Proposta("Cliente", "123", 25, 1000m);
        _propostaRepositoryMock.Setup(r => r.ObterPorIdAsync(propostaId)).ReturnsAsync(proposta);

        // Act
        var result = await _sut.ObterPropostaPorIdAsync(propostaId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(proposta.Id, result.Id);
    }

    [Fact]
    public async Task ObterPropostaPorIdAsync_Deve_RetornarNull_Quando_PropostaNaoExiste()
    {
        // Arrange
        _propostaRepositoryMock.Setup(r => r.ObterPorIdAsync(It.IsAny<Guid>())).ReturnsAsync((Proposta?)null);

        // Act
        var result = await _sut.ObterPropostaPorIdAsync(Guid.NewGuid());

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task AprovarPropostaAsync_Deve_ChamarAtualizar_Quando_PropostaPendente()
    {
        // Arrange
        var propostaId = Guid.NewGuid();
        var proposta = new Proposta("Cliente", "123", 25, 1000m); // Status inicial é Pendente
        _propostaRepositoryMock.Setup(r => r.ObterPorIdAsync(propostaId)).ReturnsAsync(proposta);
        _propostaRepositoryMock.Setup(r => r.AtualizarAsync(It.IsAny<Proposta>())).Returns(Task.CompletedTask);

        // Act
        await _sut.AprovarPropostaAsync(propostaId);

        // Assert
        _propostaRepositoryMock.Verify(r => r.AtualizarAsync(It.Is<Proposta>(p => p.Status == PropostaStatus.Aprovada)), Times.Once);
    }

    [Fact]
    public async Task AprovarPropostaAsync_Deve_LancarExcecao_Quando_PropostaNaoPendente()
    {
        // Arrange
        var propostaId = Guid.NewGuid();
        var proposta = new Proposta("Cliente", "123", 25, 1000m);
        proposta.Aprovar(); // Status agora é Aprovada
        _propostaRepositoryMock.Setup(r => r.ObterPorIdAsync(propostaId)).ReturnsAsync(proposta);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => _sut.AprovarPropostaAsync(propostaId));
    }
    
    [Fact]
    public async Task RecusarPropostaAsync_Deve_ChamarAtualizar_Quando_PropostaPendente()
    {
        // Arrange
        var propostaId = Guid.NewGuid();
        var proposta = new Proposta("Cliente", "123", 25, 1000m); // Status inicial é Pendente
        _propostaRepositoryMock.Setup(r => r.ObterPorIdAsync(propostaId)).ReturnsAsync(proposta);
        _propostaRepositoryMock.Setup(r => r.AtualizarAsync(It.IsAny<Proposta>())).Returns(Task.CompletedTask);

        // Act
        await _sut.RecusarPropostaAsync(propostaId);

        // Assert
        _propostaRepositoryMock.Verify(r => r.AtualizarAsync(It.Is<Proposta>(p => p.Status == PropostaStatus.Recusada)), Times.Once);
    }
}
