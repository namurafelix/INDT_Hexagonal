using Moq;
using ContratacaoService.Application.Services;
using ContratacaoService.Domain.Entities;
using ContratacaoService.Domain.Repositories;
using System;
using System.Threading.Tasks;
using Xunit;

namespace ContratacaoService.UnitTests;

public class ContratacaoAppServiceTests
{
    private readonly Mock<IContratacaoRepository> _contratacaoRepositoryMock;
    private readonly Mock<IPropostaServiceClient> _propostaServiceClientMock;
    private readonly ContratacaoAppService _sut;

    public ContratacaoAppServiceTests()
    {
        _contratacaoRepositoryMock = new Mock<IContratacaoRepository>();
        _propostaServiceClientMock = new Mock<IPropostaServiceClient>();
        _sut = new ContratacaoAppService(_contratacaoRepositoryMock.Object, _propostaServiceClientMock.Object);
    }

    [Fact]
    public async Task ContratarAsync_Deve_AdicionarContratacao_Quando_PropostaAprovada()
    {
        // Arrange
        var propostaId = Guid.NewGuid();
        _propostaServiceClientMock.Setup(c => c.ObterStatusPropostaAsync(propostaId)).ReturnsAsync("Aprovada");
        _contratacaoRepositoryMock.Setup(r => r.AdicionarAsync(It.IsAny<Contratacao>())).Returns(Task.CompletedTask);

        // Act
        await _sut.ContratarAsync(propostaId);

        // Assert
        _contratacaoRepositoryMock.Verify(r => r.AdicionarAsync(It.Is<Contratacao>(c => c.PropostaId == propostaId)), Times.Once);
    }

    [Fact]
    public async Task ContratarAsync_Deve_LancarExcecao_Quando_PropostaNaoAprovada()
    {
        // Arrange
        var propostaId = Guid.NewGuid();
        _propostaServiceClientMock.Setup(c => c.ObterStatusPropostaAsync(propostaId)).ReturnsAsync("Pendente");

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => _sut.ContratarAsync(propostaId));
    }

    [Fact]
    public async Task ContratarAsync_Deve_LancarExcecao_Quando_PropostaNaoEncontrada()
    {
        // Arrange
        var propostaId = Guid.NewGuid();
        _propostaServiceClientMock.Setup(c => c.ObterStatusPropostaAsync(propostaId)).ThrowsAsync(new Exception("Proposta não encontrada"));

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => _sut.ContratarAsync(propostaId));
    }
}
