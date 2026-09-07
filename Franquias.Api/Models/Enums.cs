namespace Franquias.Api.Models
{
    public enum Perfil
    {
        AdministradorFranqueadora = 1,
        GestorUnidade = 2,
        Operador = 3
    }

    public enum TipoMovimentacao
    {
        Entrada = 1,
        Saida = 2
    }

    public enum StatusVenda
    {
        Aberta = 1,
        Confirmada = 2,
        Cancelada = 3
    }

    public enum StatusPagamento
    {
        Pendente = 1,
        Pago = 2
    }

    public enum PrioridadeChamado
    {
        Baixa = 1,
        Media = 2,
        Alta = 3
    }

    public enum StatusChamado
    {
        Aberto = 1,
        EmAndamento = 2,
        Encerrado = 3
    }
}
