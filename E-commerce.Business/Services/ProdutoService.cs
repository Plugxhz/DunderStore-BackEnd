using Codigo_De_Barra.Data.Repositories;
using Codigo_De_Barra.Entities;
using Codigo_De_Barra.Interfaces.IRepositories;
using Codigo_De_Barra.Interfaces.IServices;

namespace Codigo_De_Barra.Services
{
    public class ProdutoService : IProdutoService
    {
        private readonly IProdutoRepository _produtoRepository;

        public ProdutoService(IProdutoRepository produtoRepository)
        {
            _produtoRepository = produtoRepository;
        }

        public async Task<IEnumerable<Produto>> GetAllAsync()
        {
            return await _produtoRepository.GetAllAsync();
        }

        public async Task<Produto?> GetByIdAsync(string id)
        {
            return await _produtoRepository.GetByIdAsync(id);
        }

        public async Task<Produto> CriarProdutoAsync(Produto produto)
        {
            await _produtoRepository.AddAsync(produto);
            return produto;
        }

        public async Task AtualizarProdutoAsync(Produto produto)
        {
            await _produtoRepository.UpdateAsync(produto);
        }

        public async Task RemoverProdutoAsync(string id)
        {
            var produto = await _produtoRepository.GetByIdAsync(id);
            if (produto != null)
            {
                await _produtoRepository.DeleteAsync(produto);
            }
        }
    }
}
