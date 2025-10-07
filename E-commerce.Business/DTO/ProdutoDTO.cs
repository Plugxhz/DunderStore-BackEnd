namespace Dunder_Store.DTO
{
    public class ProdutoDTO
    {
        public string nome { get; set; }
        public string descricao { get; set; }
        public decimal preco { get; set; }
        public string codigoDeBarra { get; set; }
        public string? cor { get; set; }
        public string? tamanho { get; set; }
        public string? produtoPaiId { get; set; } // null = produto principal
    }
}
