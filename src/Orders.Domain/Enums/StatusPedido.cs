namespace Orders.Domain.Enums;
public enum StatusPedido
{
    Criado = 0,
    AguardandoConfirmacao = 1,
    Confirmado = 2,
    Rejeitado = 3,
    Cancelado = 4,
    EmPreparacao = 5,
    Pronto = 6,
    Finalizado = 7
}
