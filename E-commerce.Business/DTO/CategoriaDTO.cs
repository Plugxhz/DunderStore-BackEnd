using System.ComponentModel.DataAnnotations;

namespace Dunder_Store.DTO
{
    public class CategoriaDTO
    {
        [Required(ErrorMessage = "O nome da categoria é obrigatório.")]
        public string Nome { get; set; } = string.Empty;

        // Recebe do form-data como string
        public string? CategoriaPaiId { get; set; }
    }
}
