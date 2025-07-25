namespace ContratacaoService.Application.Services;

public interface IPropostaServiceClient
{
    Task<string> ObterStatusPropostaAsync(Guid propostaId);
}
