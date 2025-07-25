using PropostaService.Domain.Enums;

namespace PropostaService.Domain.Entities;

public class Proposta
{
    public Guid Id { get; private set; }
    public string NomeCliente { get; private set; }
    public string CPF { get; private set; }
    public int Idade { get; private set; }
    public decimal ValorSeguro { get; private set; }
    public PropostaStatus Status { get; private set; }

    // Construtor para EF Core
    protected Proposta() { }

    public Proposta(string nomeCliente, string cpf, int idade, decimal valorSeguro)
    {
        Id = Guid.NewGuid();
        NomeCliente = nomeCliente;
        CPF = cpf;
        Idade = idade;
        ValorSeguro = valorSeguro;
        Status = PropostaStatus.Pendente;
    }

    public void Aprovar()
    {
        if (Status == PropostaStatus.Pendente)
        {
            Status = PropostaStatus.Aprovada;
        }
        else
        {
            throw new InvalidOperationException("A proposta não pode ser aprovada neste status.");
        }
    }

    public void Recusar()
    {
        if (Status == PropostaStatus.Pendente)
        {
            Status = PropostaStatus.Recusada;
        }
        else
        {
            throw new InvalidOperationException("A proposta não pode ser recusada neste status.");
        }
    }
}
