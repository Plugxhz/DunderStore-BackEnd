using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Dunder_Store.Entities
{
    public class Categoria
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required(ErrorMessage = "O nome da categoria é obrigatório.")]
        public string Nome { get; set; } = string.Empty;

        public Guid? CategoriaPaiId { get; set; }
        public Categoria? CategoriaPai { get; set; }

        public ICollection<Categoria>? Subcategorias { get; set; } = new List<Categoria>();
        public ICollection<Produto>? Produtos { get; set; } = new List<Produto>();
    }
}
