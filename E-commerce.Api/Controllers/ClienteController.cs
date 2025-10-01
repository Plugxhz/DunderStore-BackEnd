using Dunder_Store.Database;
using Dunder_Store.DTO;
using Dunder_Store.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Dunder_Store.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClienteController : ControllerBase
    {
        private readonly ProdutosDbContext dbContext;
        public ClienteController(ProdutosDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        [HttpGet]
        public ActionResult<IEnumerable<ClienteDTOOutput>> GetCliente()
        {
            IEnumerable<ClienteDTOOutput> clientes = dbContext
                .Clientes
                .Select(
                    c => new ClienteDTOOutput(c.Id, c.Nome, c.Cpf, c.Email)
                );

            return Ok(clientes);
        }


        [HttpGet("{id}")]
        public ActionResult<Cliente> GetClienteId(string id)
        {
            Cliente? cliente = dbContext
                .Clientes
                .FirstOrDefault(c => c.Id == id);
            if (cliente is null)
            {
                return NotFound();
            }

            ClienteDTOOutput clienteDTO = new ClienteDTOOutput(cliente.Id, cliente.Nome, cliente.Cpf, cliente.Email);

            return Ok(cliente);
        }


        [HttpPost]
        public ActionResult<ClienteDTOInput> CreateCliente(ClienteDTOInput novoClienteDTO)
        {
            if (dbContext.Clientes.Any(cliente => cliente.Cpf.Equals(novoClienteDTO.cpf)))
            {
                return BadRequest("Já existe um cliente com este CPF");
            }

            Cliente novoCliente = new Cliente(novoClienteDTO.nome, novoClienteDTO.cpf, novoClienteDTO.email, novoClienteDTO.senha); 

            dbContext.Clientes.Add(novoCliente);
            dbContext.SaveChanges();

            return CreatedAtAction(nameof(CreateCliente), novoCliente);
        }


        [HttpPost("login")]
        public ActionResult<Cliente> LoginCliente(LoginDTO loginDTO)
        {
            Cliente? cliente = dbContext
                .Clientes
                .FirstOrDefault(c => c.Email == loginDTO.email && c.Senha == loginDTO.senha);
            if (cliente is null)
            {
                return Unauthorized("Email ou senha inválidos");
            }

            ClienteDTOOutput clienteDTO = new ClienteDTOOutput(cliente.Id, cliente.Nome, cliente.Cpf, cliente.Email);
            
            return Ok(clienteDTO);
        }


        [HttpPatch("{id}")]
        public IActionResult UpdateCliente(string id, ClienteDTOInput clienteAtualizadoDTO)
        {
            Cliente? clienteEncontrado =
                dbContext
                .Clientes
                .FirstOrDefault(c => c.Id == id);

            if (clienteEncontrado is null)
            {
                return NotFound();
            }
            if (dbContext.Clientes.Any(cliente => cliente.Id != id && cliente.Cpf.Equals(clienteAtualizadoDTO.cpf)))
            {
                return BadRequest("Já existe um cliente com esse CPF");
            }

            // Se o valor for null não altera
            if (clienteAtualizadoDTO.nome != "string") { clienteEncontrado.Nome = clienteAtualizadoDTO.nome; }
            if (clienteAtualizadoDTO.cpf != "string") { clienteEncontrado.Cpf = clienteAtualizadoDTO.cpf; }
            if (clienteAtualizadoDTO.email != "string") { clienteEncontrado.Email = clienteAtualizadoDTO.email; }
            if (clienteAtualizadoDTO.senha != "string") { clienteEncontrado.Senha = clienteAtualizadoDTO.senha; }

            dbContext.SaveChanges();

            return NoContent();
        }


        [HttpDelete("{id}")]
        public IActionResult DeleteCliente(string id)
        {
            Cliente? clienteEncontrado =
                dbContext
                .Clientes
                .Include(c => c.Pedidos)
                    .ThenInclude(p => p.PedidoProdutos) // Inclui os produtos dos pedidos do cliente
                .FirstOrDefault(c => c.Id == id);

            if (clienteEncontrado == null)
            {
                return NotFound();
            }

            foreach (Pedido pedido in clienteEncontrado.Pedidos)
            {
                pedido.PedidoProdutos.Clear(); // Limpa os produtos do pedido antes de remover
                dbContext.Pedidos.Remove(pedido);
            }

            dbContext.Clientes.Remove(clienteEncontrado);
            dbContext.SaveChanges();

            return NoContent();
        }
    }
}
