using Dunder_Store.Database;
using Dunder_Store.DTO;
using Dunder_Store.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Dunder_Store.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PedidoController : ControllerBase
    {
        private readonly ProdutosDbContext dbContext;

        public PedidoController(ProdutosDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Pedido>>> GetPedidos()
        {
            var pedidos = await dbContext.Pedidos
                .Include(p => p.Cliente)
                .Include(p => p.PedidoProdutos).ThenInclude(pp => pp.Produto)
                .OrderByDescending(p => p.DataPedido)
                .ToListAsync();

            if (!pedidos.Any())
                return NoContent();

            return Ok(pedidos);
        }

        [HttpGet("{id:guid}")]
        public async Task<ActionResult<Pedido>> GetPedido(Guid id)
        {
            var pedido = await dbContext.Pedidos
                .Include(p => p.PedidoProdutos).ThenInclude(pp => pp.Produto)
                .Include(p => p.Cliente)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (pedido == null)
                return NotFound("Pedido não encontrado.");

            return Ok(pedido);
        }

        [HttpGet("cliente/{clienteId:guid}")]
        public async Task<ActionResult<IEnumerable<Pedido>>> GetPedidosPorCliente(Guid clienteId, [FromQuery] PedidoStatus? status = null)
        {
            var cliente = await dbContext.Clientes.FindAsync(clienteId);
            if (cliente == null)
                return BadRequest("Cliente não encontrado.");

            var query = dbContext.Pedidos
                .Include(p => p.PedidoProdutos).ThenInclude(pp => pp.Produto)
                .Include(p => p.Cliente)
                .Where(p => p.ClienteId == clienteId);

            if (status.HasValue)
                query = query.Where(p => p.Status == status.Value);

            var pedidos = await query.OrderByDescending(p => p.DataPedido).ToListAsync();

            if (!pedidos.Any())
                return NoContent();

            return Ok(pedidos);
        }

        [HttpPost]
        public async Task<ActionResult<Pedido>> CreatePedido(PedidoDTO novoPedidoDTO)
        {
            if (novoPedidoDTO.produtos == null || !novoPedidoDTO.produtos.Any())
                return BadRequest("É necessário enviar a lista de produtos.");

            var cliente = await dbContext.Clientes.FirstOrDefaultAsync(c => c.Cpf == novoPedidoDTO.clientecpf);
            if (cliente == null)
                return BadRequest("Cliente inválido.");

            var codigos = novoPedidoDTO.produtos.Select(p => p.CodigoDeBarra).ToList();
            var produtosEncontrados = await dbContext.Produtos.Where(p => codigos.Contains(p.CodigoDeBarra)).ToListAsync();

            if (produtosEncontrados.Count != novoPedidoDTO.produtos.Count)
                return BadRequest("Um ou mais produtos não foram encontrados.");

            var novoPedido = new Pedido(cliente, DateTime.Now)
            {
                Status = PedidoStatus.Carrinho
            };

            foreach (var produtoDTO in novoPedidoDTO.produtos)
            {
                var produto = produtosEncontrados.FirstOrDefault(p => p.CodigoDeBarra == produtoDTO.CodigoDeBarra);
                if (produto != null)
                {
                    novoPedido.PedidoProdutos.Add(new PedidoProduto
                    {
                        ProdutoId = produto.Id,
                        Produto = produto,
                        Quantidade = produtoDTO.Quantidade
                    });
                }
            }

            await dbContext.Pedidos.AddAsync(novoPedido);
            await dbContext.SaveChangesAsync();

            return CreatedAtAction(nameof(GetPedido), new { id = novoPedido.Id }, novoPedido);
        }

        [HttpGet("carrinho/{clienteId:guid}")]
        public async Task<ActionResult<Pedido>> GetCarrinhoPorCliente(Guid clienteId)
        {
            var carrinho = await dbContext.Pedidos
                .Include(p => p.PedidoProdutos).ThenInclude(pp => pp.Produto)
                .Include(p => p.Cliente)
                .FirstOrDefaultAsync(p => p.ClienteId == clienteId && p.Status == PedidoStatus.Carrinho);

            if (carrinho == null)
            {
                var cliente = await dbContext.Clientes.FindAsync(clienteId);
                if (cliente == null)
                    return NotFound("Cliente não encontrado.");

                carrinho = new Pedido(cliente, DateTime.Now)
                {
                    Status = PedidoStatus.Carrinho
                };

                await dbContext.Pedidos.AddAsync(carrinho);
                await dbContext.SaveChangesAsync();
            }

            return Ok(carrinho);
        }

        [HttpPost("finalizar/{id:guid}")]
        public async Task<IActionResult> FinalizarPedido(Guid id)
        {
            var pedido = await dbContext.Pedidos.FindAsync(id);
            if (pedido == null)
                return NotFound("Pedido não encontrado.");

            if (pedido.Status != PedidoStatus.Carrinho)
                return BadRequest("Pedido já está finalizado.");

            pedido.Status = PedidoStatus.Finalizado;
            pedido.DataPedido = DateTime.Now;

            await dbContext.SaveChangesAsync();
            return NoContent();
        }

        [HttpPut("{id:guid}")]
        public async Task<IActionResult> UpdatePedido(Guid id, PedidoDTO pedidoAtualizadoDTO)
        {
            var pedido = await dbContext.Pedidos
                .Include(p => p.PedidoProdutos).ThenInclude(pp => pp.Produto)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (pedido == null)
                return NotFound("Pedido não encontrado.");

            if (pedido.Status != PedidoStatus.Carrinho)
                return BadRequest("Não é possível alterar um pedido que já foi finalizado.");

            if (pedidoAtualizadoDTO.produtos == null || !pedidoAtualizadoDTO.produtos.Any())
                return BadRequest("É necessário enviar a lista de produtos.");

            dbContext.PedidoProdutos.RemoveRange(pedido.PedidoProdutos);
            pedido.PedidoProdutos.Clear();

            var codigos = pedidoAtualizadoDTO.produtos.Select(p => p.CodigoDeBarra).ToList();
            var produtosEncontrados = await dbContext.Produtos.Where(p => codigos.Contains(p.CodigoDeBarra)).ToListAsync();

            foreach (var produtoDTO in pedidoAtualizadoDTO.produtos)
            {
                var produto = produtosEncontrados.FirstOrDefault(p => p.CodigoDeBarra == produtoDTO.CodigoDeBarra);
                if (produto != null)
                {
                    pedido.PedidoProdutos.Add(new PedidoProduto
                    {
                        Produto = produto,
                        Quantidade = produtoDTO.Quantidade
                    });
                }
            }

            await dbContext.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> DeletePedido(Guid id)
        {
            var pedido = await dbContext.Pedidos
                .Include(p => p.PedidoProdutos)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (pedido == null)
                return NotFound("Pedido não encontrado.");

            dbContext.PedidoProdutos.RemoveRange(pedido.PedidoProdutos);
            dbContext.Pedidos.Remove(pedido);
            await dbContext.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("cliente/{clienteId:guid}")]
        public async Task<IActionResult> DeletePedidosCliente(Guid clienteId)
        {
            var cliente = await dbContext.Clientes.FindAsync(clienteId);
            if (cliente == null)
                return BadRequest("Cliente não encontrado.");

            var pedidos = await dbContext.Pedidos
                .Include(p => p.PedidoProdutos)
                .Where(p => p.ClienteId == clienteId)
                .ToListAsync();

            if (!pedidos.Any())
                return NoContent();

            foreach (var pedido in pedidos)
            {
                dbContext.PedidoProdutos.RemoveRange(pedido.PedidoProdutos);
                dbContext.Pedidos.Remove(pedido);
            }

            await dbContext.SaveChangesAsync();
            return NoContent();
        }
    }
}
