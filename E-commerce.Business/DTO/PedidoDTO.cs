using Dunder_Store.Entities;

namespace Dunder_Store.DTO
{
    public class PedidoDTO
    {
        public string clientecpf { get; set; }
        public List<PedidoProdutoDTO> produtos { get; set; }
    }

}
