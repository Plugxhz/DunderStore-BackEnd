using Codigo_De_Barra.Database;
using Codigo_De_Barra.Entities;
using Codigo_De_Barra.Interfaces.IRepositories;
using Microsoft.EntityFrameworkCore;

namespace Codigo_De_Barra.Data.Repositories
{
    public class ProdutoRepository : IProdutoRepository
    {
        private readonly ProdutosDbContext _dbContext;

        public ProdutoRepository(ProdutosDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IEnumerable<Produto>> GetAllAsync()
        {
            return await _dbContext.Produtos.ToListAsync();
        }

        public async Task<Produto?> GetByIdAsync(string id)
        {
            return await _dbContext.Produtos.FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task AddAsync(Produto produto)
        {
            await _dbContext.Produtos.AddAsync(produto);
            await _dbContext.SaveChangesAsync();
        }

        public async Task UpdateAsync(Produto produto)
        {
            _dbContext.Produtos.Update(produto);
            await _dbContext.SaveChangesAsync();
        }

        public async Task DeleteAsync(Produto produto)
        {
            _dbContext.Produtos.Remove(produto);
            await _dbContext.SaveChangesAsync();
        }
    }
}
