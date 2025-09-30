using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Codigo_De_Barra.Migrations
{
    /// <inheritdoc />
    public partial class teste : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Produtos_Pedidos_Pedidoid",
                table: "Produtos");

            migrationBuilder.DropColumn(
                name: "NomeCliente",
                table: "Pedidos");

            migrationBuilder.RenameColumn(
                name: "Pedidoid",
                table: "Produtos",
                newName: "PedidoId");

            migrationBuilder.RenameIndex(
                name: "IX_Produtos_Pedidoid",
                table: "Produtos",
                newName: "IX_Produtos_PedidoId");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "Pedidos",
                newName: "Id");

            migrationBuilder.AddColumn<string>(
                name: "NomeClienteId",
                table: "Pedidos",
                type: "varchar(255)",
                nullable: true,
                collation: "utf8mb4_0900_ai_ci")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Cliente",
                columns: table => new
                {
                    Id = table.Column<string>(type: "varchar(255)", nullable: false, collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Nome = table.Column<string>(type: "longtext", nullable: false, collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Cpf = table.Column<string>(type: "longtext", nullable: false, collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Email = table.Column<string>(type: "longtext", nullable: false, collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Senha = table.Column<string>(type: "longtext", nullable: false, collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cliente", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4")
                .Annotation("Relational:Collation", "utf8mb4_0900_ai_ci");

            migrationBuilder.CreateIndex(
                name: "IX_Pedidos_NomeClienteId",
                table: "Pedidos",
                column: "NomeClienteId");

            migrationBuilder.AddForeignKey(
                name: "FK_Pedidos_Cliente_NomeClienteId",
                table: "Pedidos",
                column: "NomeClienteId",
                principalTable: "Cliente",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Produtos_Pedidos_PedidoId",
                table: "Produtos",
                column: "PedidoId",
                principalTable: "Pedidos",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Pedidos_Cliente_NomeClienteId",
                table: "Pedidos");

            migrationBuilder.DropForeignKey(
                name: "FK_Produtos_Pedidos_PedidoId",
                table: "Produtos");

            migrationBuilder.DropTable(
                name: "Cliente");

            migrationBuilder.DropIndex(
                name: "IX_Pedidos_NomeClienteId",
                table: "Pedidos");

            migrationBuilder.DropColumn(
                name: "NomeClienteId",
                table: "Pedidos");

            migrationBuilder.RenameColumn(
                name: "PedidoId",
                table: "Produtos",
                newName: "Pedidoid");

            migrationBuilder.RenameIndex(
                name: "IX_Produtos_PedidoId",
                table: "Produtos",
                newName: "IX_Produtos_Pedidoid");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "Pedidos",
                newName: "id");

            migrationBuilder.AddColumn<string>(
                name: "NomeCliente",
                table: "Pedidos",
                type: "longtext",
                nullable: false,
                collation: "utf8mb4_0900_ai_ci")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddForeignKey(
                name: "FK_Produtos_Pedidos_Pedidoid",
                table: "Produtos",
                column: "Pedidoid",
                principalTable: "Pedidos",
                principalColumn: "id");
        }
    }
}
