using Dunder_Store.Entities;
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
        public Cliente Cliente { get; set; }

        [Required]
        public DateTime DataPedido { get; set; }

        // RELACIONAMENTO N:N via PedidoProduto
        public List<PedidoProduto> PedidoProdutos { get; set; } = new();

        [NotMapped]
        public decimal ValorTotal => PedidoProdutos.Sum(pp => pp.Produto.Preco);

        public Pedido(Cliente cliente, DateTime data)
        {
            Cliente = cliente;
            DataPedido = data;
        }

        private Pedido() { }
    }
}
