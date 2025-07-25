namespace PropostaService.Application.DTOs;

public record PropostaViewModel(Guid Id, string NomeCliente, string CPF, int Idade, decimal ValorSeguro, string Status);
