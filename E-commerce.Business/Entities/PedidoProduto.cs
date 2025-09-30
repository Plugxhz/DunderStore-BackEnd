using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Codigo_De_Barra.Entities
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
