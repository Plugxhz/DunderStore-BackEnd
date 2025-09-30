namespace Codigo_De_Barra.DTO
{
    public class ClienteDTOInput
    {
        public string nome { get; set; }
        public string cpf { get; set; }
        public string email { get; set; }
        public string senha { get; set; }
    }

    public class ClienteDTOOutput
    {
        public string Id { get; set; }
        public string Nome { get; set; }
        public string Cpf { get; set; }
        public string Email { get; set; }
        public string Senha { get; set; }

        public ClienteDTOOutput(string id, string nome, string cpf, string email) 
        {
            this.Id = id;
            this.Nome = nome;
            this.Cpf = cpf;
            this.Email = email;
        }

    }
}
