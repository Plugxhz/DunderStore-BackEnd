using Codigo_De_Barra.Entities;

namespace Codigo_De_Barra.Interfaces.IServices
{
    public interface IProdutoService
    {
        Task<IEnumerable<Produto>> GetAllAsync();
        Task<Produto?> GetByIdAsync(string id);
        Task<Produto> CriarProdutoAsync(Produto produto);
        Task AtualizarProdutoAsync(Produto produto);
        Task RemoverProdutoAsync(string id);
    }
}
