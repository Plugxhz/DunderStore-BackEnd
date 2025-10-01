using Dunder_Store.Entities;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class Pedido
{
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Key]
    public string Id { get; set; }
    public Cliente Cliente { get; set; }
    public DateTime DataPedido { get; set; }

    public List<PedidoProduto> PedidoProdutos { get; set; } = new();

    public decimal ValorTotal
    {
        get
        {
            return PedidoProdutos.Sum(pp => pp.Produto.Preco);
        }
    }

    public Pedido(Cliente cliente, DateTime data)
    {
        this.Cliente = cliente;
        this.DataPedido = data;
    }

    private Pedido() { }
}
