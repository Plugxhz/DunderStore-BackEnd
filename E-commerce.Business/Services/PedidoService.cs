using Codigo_De_Barra.Entities;
using Codigo_De_Barra.Interfaces.IRepositories;
using Codigo_De_Barra.Interfaces.IServices;

namespace Codigo_De_Barra.Services
{
    public class PedidoService : IPedidoService
    {
        private readonly IPedidoRepository _pedidoRepository;

        public PedidoService(IPedidoRepository pedidoRepository)
        {
            _pedidoRepository = pedidoRepository;
        }

        public async Task<IEnumerable<Pedido>> GetAllAsync()
        {
            return await _pedidoRepository.GetAllAsync();
        }

        public async Task<Pedido?> GetByIdAsync(string id)
        {
            return await _pedidoRepository.GetByIdAsync(id);
        }

        public async Task<Pedido> CriarPedidoAsync(Pedido pedido)
        {
            await _pedidoRepository.AddAsync(pedido);
            return pedido;
        }

        public async Task AtualizarPedidoAsync(Pedido pedido)
        {
            await _pedidoRepository.UpdateAsync(pedido);
        }

        public async Task RemoverPedidoAsync(string id)
        {
            var pedido = await _pedidoRepository.GetByIdAsync(id);
            if (pedido != null)
            {
                await _pedidoRepository.DeleteAsync(pedido);
            }
        }
    }
}
