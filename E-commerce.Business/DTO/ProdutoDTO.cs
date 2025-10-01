using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Dunder_Store.DTO
{
    public class ProdutoDTO
    {
        public string nome { get; set; }
        public string descricao { get; set; }
        public decimal preco { get; set; }
        public string codigoDeBarra { get; set; }
    }
}
