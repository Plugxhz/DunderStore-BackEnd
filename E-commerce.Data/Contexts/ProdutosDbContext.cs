using System;
using Codigo_De_Barra.Entities;
using Microsoft.EntityFrameworkCore;

namespace Codigo_De_Barra.Database
{
    public partial class ProdutosDbContext : DbContext
    {
        public DbSet<Cliente> Clientes { get; set; }
        public DbSet<Produto> Produtos { get; set; }
        public DbSet<Pedido> Pedidos { get; set; }
        public DbSet<PedidoProduto> PedidoProdutos { get; set; }

        public ProdutosDbContext(DbContextOptions<ProdutosDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Chave composta para a tabela de junção
            modelBuilder.Entity<PedidoProduto>()
                .HasKey(pp => new { pp.PedidoId, pp.ProdutoId });

            // Relacionamento: PedidoProduto -> Pedido
            modelBuilder.Entity<PedidoProduto>()
                .HasOne(pp => pp.Pedido)
                .WithMany(p => p.PedidoProdutos)
                .HasForeignKey(pp => pp.PedidoId)
                .OnDelete(DeleteBehavior.Cascade);

            // Relacionamento: PedidoProduto -> Produto
            modelBuilder.Entity<PedidoProduto>()
                .HasOne(pp => pp.Produto)
                .WithMany(p => p.PedidoProdutos) // ← agora está correto!
                .HasForeignKey(pp => pp.ProdutoId)
                .OnDelete(DeleteBehavior.Cascade);

            // Nome da tabela no banco
            modelBuilder.Entity<PedidoProduto>()
                .ToTable("PedidoProduto");

            base.OnModelCreating(modelBuilder);
        }
    }
}
