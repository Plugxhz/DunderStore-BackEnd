using Dunder_Store.Database;
using Dunder_Store.DTO;
using Dunder_Store.Entities;
using Dunder_Store.Interfaces.IServices;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Dunder_Store.Services
{
    public class CategoriaService : ICategoriaService
    {
        private readonly ProdutosDbContext _context;

        public CategoriaService(ProdutosDbContext context)
        {
            _context = context;
        }

        public async Task<List<Categoria>> GetAllAsync()
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

        public async Task<Categoria> CriarCategoriaAsync(CategoriaDTO dto)
        {
            Guid? categoriaPaiGuid = null;
            if (!string.IsNullOrEmpty(dto.CategoriaPaiId))
                categoriaPaiGuid = Guid.Parse(dto.CategoriaPaiId);

            var categoria = new Categoria
            {
                Nome = dto.Nome,
                CategoriaPaiId = categoriaPaiGuid
            };

            await _context.Categorias.AddAsync(categoria);
            await _context.SaveChangesAsync();

            return categoria;
        }

        public async Task AtualizarCategoriaAsync(Guid id, CategoriaDTO dto)
        {
            var categoria = await _context.Categorias.FindAsync(id);
            if (categoria == null)
                throw new Exception("Categoria não encontrada.");

            categoria.Nome = dto.Nome;

            if (!string.IsNullOrEmpty(dto.CategoriaPaiId))
                categoria.CategoriaPaiId = Guid.Parse(dto.CategoriaPaiId);
            else
                categoria.CategoriaPaiId = null;

            _context.Categorias.Update(categoria);
            await _context.SaveChangesAsync();
        }

        public async Task RemoverCategoriaAsync(Guid id)
        {
            var categoria = await _context.Categorias
                .Include(c => c.Subcategorias)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (categoria == null)
                throw new Exception("Categoria não encontrada.");

            if (categoria.Subcategorias != null && categoria.Subcategorias.Any())
                throw new Exception("Não é possível excluir uma categoria que possui subcategorias.");

            _context.Categorias.Remove(categoria);
            await _context.SaveChangesAsync();
        }
    }
}
