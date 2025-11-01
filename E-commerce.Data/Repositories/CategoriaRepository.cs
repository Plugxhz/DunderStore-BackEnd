using Dunder_Store.Database;
using Dunder_Store.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Dunder_Store.Repositories
{
    public class CategoriaRepository
    {
        private readonly ProdutosDbContext _context;

        public CategoriaRepository(ProdutosDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Categoria>> GetAllAsync()
        {
            return await _context.Categorias
                .Include(c => c.Subcategorias)
                .Include(c => c.Produtos)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<Categoria?> GetByIdAsync(Guid id)
        {
            return await _context.Categorias
                .Include(c => c.Subcategorias)
                .Include(c => c.Produtos)
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task AddAsync(Categoria categoria)
        {
            await _context.Categorias.AddAsync(categoria);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Categoria categoria)
        {
            _context.Categorias.Update(categoria);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid id)
        {
            var categoria = await _context.Categorias.FindAsync(id);
            if (categoria != null)
            {
                _context.Categorias.Remove(categoria);
                await _context.SaveChangesAsync();
            }
        }
    }
}
