using Dunder_Store.Data.Repositories;
using Dunder_Store.Entities;
using Dunder_Store.Interfaces.IRepositories;
using Dunder_Store.Interfaces.IServices;

namespace Dunder_Store.Services
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
