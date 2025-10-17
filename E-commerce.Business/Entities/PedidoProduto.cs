using Dunder_Store.Entities;
using System.ComponentModel.DataAnnotations.Schema;

public class PedidoProduto
{
    public Guid PedidoId { get; set; }
    [ForeignKey(nameof(PedidoId))]
    public Pedido Pedido { get; set; }

    public Guid ProdutoId { get; set; }
    [ForeignKey(nameof(ProdutoId))]
    public Produto Produto { get; set; }

    public int Quantidade { get; set; }
}
