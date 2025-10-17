using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Dunder_Store.Entities
{
    public class Cliente
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public string Id { get; set; }
        public string Nome { get; set; } = string.Empty;
        public string Cpf { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Senha { get; set; } = string.Empty;
        public string Cep { get; set; } = string.Empty;
        public List<Pedido> Pedidos { get; set; } = new();

        public Cliente(string nome, string cpf, string email, string senha, string cep)
        {
            this.Nome = nome;
            this.Cpf = cpf;
            this.Email = email;
            this.Senha = senha;
            this.Cep = cep;
        }
        private Cliente() { }
    }
}
