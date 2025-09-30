using Codigo_De_Barra.Database;
using Codigo_De_Barra.Entities;
using Codigo_De_Barra.Interfaces.IRepositories;
using Microsoft.EntityFrameworkCore;

namespace Codigo_De_Barra.Data.Repositories
{
    public class ClienteRepository : IClienteRepository
    {
        private readonly ProdutosDbContext _dbContext;

        public ClienteRepository(ProdutosDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IEnumerable<Cliente>> GetAllAsync()
        {
            return await _dbContext.Clientes.ToListAsync();
        }

        public async Task<Cliente?> GetByIdAsync(string id)
        {
            return await _dbContext.Clientes.FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task AddAsync(Cliente cliente)
        {
            await _dbContext.Clientes.AddAsync(cliente);
            await _dbContext.SaveChangesAsync();
        }

        public async Task UpdateAsync(Cliente cliente)
        {
            _dbContext.Clientes.Update(cliente);
            await _dbContext.SaveChangesAsync();
        }

        public async Task DeleteAsync(Cliente cliente)
        {
            _dbContext.Clientes.Remove(cliente);
            await _dbContext.SaveChangesAsync();
        }
    }
}
