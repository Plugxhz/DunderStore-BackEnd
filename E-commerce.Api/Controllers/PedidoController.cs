using Dunder_Store.Database;
using Dunder_Store.DTO;
using Dunder_Store.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

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

        // [Authorize] // Exige Token Administrativo
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

        [HttpGet("{id}")]
        public async Task<ActionResult<Pedido>> GetPedido(string id)
        {
            if (!Guid.TryParse(id, out var guidId))
                return BadRequest("ID inválido.");

            var pedido = await dbContext.Pedidos
                .Include(p => p.PedidoProdutos).ThenInclude(pp => pp.Produto)
                .Include(p => p.Cliente)
                .FirstOrDefaultAsync(p => p.Id == guidId);

            if (pedido == null)
                return NotFound("Pedido não encontrado.");

            return Ok(pedido);
        }

        [HttpGet("cliente/{idCliente}")]
        public async Task<ActionResult<IEnumerable<Pedido>>> GetPedidosPorCliente(string idCliente)
        {
            if (!Guid.TryParse(idCliente, out var guidClienteId))
                return BadRequest("ID de cliente inválido.");

            var cliente = await dbContext.Clientes.FirstOrDefaultAsync(c => c.Id.Equals(guidClienteId));
            if (cliente == null)
                return BadRequest("Cliente não encontrado.");

            var pedidos = await dbContext.Pedidos
                .Include(p => p.PedidoProdutos).ThenInclude(pp => pp.Produto)
                .Include(p => p.Cliente)
                .Where(p => p.Cliente.Id.Equals(guidClienteId))
                .OrderByDescending(p => p.DataPedido)
                .ToListAsync();

            if (!pedidos.Any())
                return NoContent();

            return Ok(pedidos);
        }

        [HttpPost]
        // [Authorize] // Exige Token Administrativo
        public async Task<ActionResult<Pedido>> CreatePedido(PedidoDTO novoPedidoDTO)
        {
            // 1️⃣ Validação básica
            if (novoPedidoDTO.produtos == null || !novoPedidoDTO.produtos.Any())
                return BadRequest("É necessário enviar a lista de produtos.");

            // 2️⃣ Buscar cliente
            var cliente = await dbContext.Clientes.FirstOrDefaultAsync(c => c.Cpf == novoPedidoDTO.clientecpf);
            if (cliente == null)
                return BadRequest("Cliente inválido.");

            // 3️⃣ Buscar produtos pelo código de barras
            var codigos = novoPedidoDTO.produtos.Select(p => p.CodigoDeBarra).ToList();

            var produtosEncontrados = await dbContext.Produtos
                .Where(p => codigos.Contains(p.CodigoDeBarra))
                .ToListAsync();

            if (produtosEncontrados.Count != novoPedidoDTO.produtos.Count)
                return BadRequest("Um ou mais produtos não foram encontrados.");

            // 4️⃣ Criar pedido e associar produtos
            var novoPedido = new Pedido(cliente, DateTime.Now);

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

            // 5️⃣ Persistir no banco
            await dbContext.Pedidos.AddAsync(novoPedido);
            await dbContext.SaveChangesAsync();

            // 6️⃣ Retornar pedido criado
            return CreatedAtAction(nameof(GetPedido), new { id = novoPedido.Id }, novoPedido);
        }

        // [Authorize] // Exige Token Administrativo
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdatePedido(string id, PedidoDTO pedidoAtualizadoDTO)
        {
            if (!Guid.TryParse(id, out var guidId))
                return BadRequest("ID inválido.");

            var pedido = await dbContext.Pedidos
                .Include(p => p.PedidoProdutos).ThenInclude(pp => pp.Produto)
                .FirstOrDefaultAsync(p => p.Id == guidId);

            if (pedido == null)
                return NotFound("Pedido não encontrado.");

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


        // [Authorize] // Exige Token Administrativo
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePedido(string id)
        {
            if (!Guid.TryParse(id, out var guidId))
                return BadRequest("ID inválido.");

            var pedido = await dbContext.Pedidos
                .Include(p => p.PedidoProdutos)
                .FirstOrDefaultAsync(p => p.Id == guidId);

            if (pedido == null)
                return NotFound("Pedido não encontrado.");

            dbContext.PedidoProdutos.RemoveRange(pedido.PedidoProdutos);
            dbContext.Pedidos.Remove(pedido);
            await dbContext.SaveChangesAsync();

            return NoContent();
        }

        // [Authorize] // Exige Token Administrativo
        [HttpDelete("cliente/{idCliente}")]
        public async Task<IActionResult> DeletePedidosCliente(string idCliente)
        {
            if (!Guid.TryParse(idCliente, out var guidClienteId))
                return BadRequest("ID de cliente inválido.");

            var cliente = await dbContext.Clientes.FirstOrDefaultAsync(c => c.Id.Equals(guidClienteId));
            if (cliente == null)
                return BadRequest("Cliente não encontrado.");

            var pedidos = await dbContext.Pedidos
                .Include(p => p.PedidoProdutos)
                .Where(p => p.Cliente.Id.Equals(guidClienteId))
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
