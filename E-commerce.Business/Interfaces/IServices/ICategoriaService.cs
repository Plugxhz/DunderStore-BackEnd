using Dunder_Store.DTO;
using Dunder_Store.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Dunder_Store.Interfaces.IServices
{
    public interface ICategoriaService
    {
        Task<List<Categoria>> GetAllAsync();
        Task<Categoria?> GetByIdAsync(Guid id);
        Task<Categoria> CriarCategoriaAsync(CategoriaDTO dto);
        Task AtualizarCategoriaAsync(Guid id, CategoriaDTO dto);
        Task RemoverCategoriaAsync(Guid id);
    }
}
