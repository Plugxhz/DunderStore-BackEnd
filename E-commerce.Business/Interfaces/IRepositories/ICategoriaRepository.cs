using Dunder_Store.Entities;

namespace Dunder_Store.Interfaces.IRepositories
{
    public interface ICategoriaRepository
    {
        Task AddAsync(Categoria categoria);
        Task<Categoria?> GetByIdAsync(Guid id);
    }
}
