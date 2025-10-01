using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Dunder_Store.Entities
{
    public class PedidoProduto
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public string Id { get; set; }

        [Required]
        public string PedidoId { get; set; }
        public Pedido Pedido { get; set; }

        [Required]
        public string ProdutoId { get; set; }
        public Produto Produto { get; set; }
    }
}
