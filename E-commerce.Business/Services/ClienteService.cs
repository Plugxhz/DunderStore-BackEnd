using Codigo_De_Barra.Entities;
using Codigo_De_Barra.Interfaces.IRepositories;
using Codigo_De_Barra.Interfaces.IServices;

namespace Codigo_De_Barra.Services
{
    public class ClienteService : IClienteService
    {
        private readonly IClienteRepository _clienteRepository;

        public ClienteService(IClienteRepository clienteRepository)
        {
            _clienteRepository = clienteRepository;
        }

        public async Task<IEnumerable<Cliente>> GetAllAsync()
        {
            return await _clienteRepository.GetAllAsync();
        }

        public async Task<Cliente?> GetByIdAsync(string id)
        {
            return await _clienteRepository.GetByIdAsync(id);
        }

        public async Task<Cliente> CriarClienteAsync(Cliente cliente)
        {
            await _clienteRepository.AddAsync(cliente);
            return cliente;
        }

        public async Task AtualizarClienteAsync(Cliente cliente)
        {
            await _clienteRepository.UpdateAsync(cliente);
        }

        public async Task RemoverClienteAsync(string id)
        {
            var cliente = await _clienteRepository.GetByIdAsync(id);
            if (cliente != null)
            {
                await _clienteRepository.DeleteAsync(cliente);
            }
        }
    }
}
