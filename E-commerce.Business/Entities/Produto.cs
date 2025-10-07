using Dunder_Store.Entities;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class Produto
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public string Id { get; set; }

    [Required]
    public string Nome { get; set; }

    public string Descricao { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal Preco { get; set; }

    public string? ImagemURL { get; set; }

    [Required]
    public string CodigoDeBarra { get; set; }

    // 🔹 Campos para variação
    public string? Cor { get; set; }
    public string? Tamanho { get; set; }

    // 🔹 Relacionamento com o produto "pai"
    public string? ProdutoPaiId { get; set; }

    [ForeignKey("ProdutoPaiId")]
    public Produto? ProdutoPai { get; set; }

    public ICollection<Produto>? Variacoes { get; set; }

    public ICollection<PedidoProduto> PedidoProdutos { get; set; }

    public Produto(string nome, string descricao, decimal preco, string codigoDeBarra, string imagemURL, string? cor = null, string? tamanho = null, string? produtoPaiId = null)
    {
        Nome = nome;
        Descricao = descricao;
        Preco = preco;
        CodigoDeBarra = codigoDeBarra;
        ImagemURL = imagemURL;
        Cor = cor;
        Tamanho = tamanho;
        ProdutoPaiId = produtoPaiId;
    }

    private Produto() { }
}
