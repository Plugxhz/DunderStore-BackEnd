using Dunder_Store.Entities;

namespace Dunder_Store.Interfaces.IServices
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
