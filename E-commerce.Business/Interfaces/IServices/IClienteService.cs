using Codigo_De_Barra.Entities;

namespace Codigo_De_Barra.Interfaces.IServices
{
    public interface IClienteService
    {
        Task<IEnumerable<Cliente>> GetAllAsync();
        Task<Cliente?> GetByIdAsync(string id);
        Task<Cliente> CriarClienteAsync(Cliente cliente);
        Task AtualizarClienteAsync(Cliente cliente);
        Task RemoverClienteAsync(string id);
    }
}
