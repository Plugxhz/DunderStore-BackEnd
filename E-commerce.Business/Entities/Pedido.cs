using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Dunder_Store.Entities
{
    public class Pedido
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        public Guid ClienteId { get; set; }

        [ForeignKey(nameof(ClienteId))]
        public Cliente Cliente { get; set; } = null!;

        [Required]
        public DateTime DataPedido { get; set; }

        [Required]
        public PedidoStatus Status { get; set; } = PedidoStatus.Carrinho;

        public List<PedidoProduto> PedidoProdutos { get; set; } = new();

        [NotMapped]
        public decimal ValorTotal => PedidoProdutos.Sum(pp => pp.Produto.Preco * pp.Quantidade);

        public Pedido(Cliente cliente, DateTime data)
        {
            Cliente = cliente;
            ClienteId = cliente.Id;
            DataPedido = data;
            Status = PedidoStatus.Carrinho;
        }

        private Pedido() { }
    }
}
