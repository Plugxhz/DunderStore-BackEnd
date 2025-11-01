﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Dunder_Store.Entities
{
    public class PedidoProduto
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        public Guid PedidoId { get; set; }

        [ForeignKey(nameof(PedidoId))]
        public Pedido Pedido { get; set; }

        [Required]
        public Guid ProdutoId { get; set; }

        [ForeignKey(nameof(ProdutoId))]
        public Produto Produto { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "A quantidade deve ser maior que zero.")]
        public int Quantidade { get; set; }

        [NotMapped]
        public decimal ValorTotal => Produto.Preco * Quantidade;
    }
}
