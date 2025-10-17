using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Dunder_Store.Migrations
{
    /// <inheritdoc />
    public partial class AddQuantityPedidoproduto : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Quantidade",
                table: "PedidoProdutos",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Quantidade",
                table: "PedidoProdutos");
        }
    }
}
