using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Codigo_De_Barra.DTO
{
    public class ProdutoDTO
    {
        public string nome { get; set; }
        public string descricao { get; set; }
        public decimal preco { get; set; }
        public string codigoDeBarra { get; set; }
    }
}
