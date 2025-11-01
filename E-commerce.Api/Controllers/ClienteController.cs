using Dunder_Store.DTO;
using Dunder_Store.Entities;
using Dunder_Store.Interfaces.IServices;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace Dunder_Store.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClienteController : ControllerBase
    {
        private readonly IClienteService _clienteService;

        public ClienteController(IClienteService clienteService)
        {
            _clienteService = clienteService;
        }

        [HttpGet]
        public async Task<ActionResult<Paginador<ClienteDTOOutput>>> GetClientes(
            [FromQuery] int pagina = 1,
            [FromQuery] int tamanhoPagina = 10)
        {
            if (pagina <= 0) pagina = 1;
            if (tamanhoPagina <= 0) tamanhoPagina = 10;

            var clientes = await _clienteService.GetAllAsync();
            var totalItens = clientes.Count();

            var clientesPaginados = clientes
                .Skip((pagina - 1) * tamanhoPagina)
                .Take(tamanhoPagina)
                .Select(c => new ClienteDTOOutput(c.Id, c.Nome, c.Cpf, c.Email, c.Cep))
                .ToList();

            if (!clientesPaginados.Any())
                return NoContent();

            var resultado = new Paginador<ClienteDTOOutput>(
                clientesPaginados,
                totalItens,
                pagina,
                tamanhoPagina
            );

            return Ok(resultado);
        }

        [HttpGet("{id:guid}")]
        public async Task<ActionResult<ClienteDTOOutput>> GetClienteId(Guid id)
        {
            var cliente = await _clienteService.GetByIdAsync(id);
            if (cliente == null)
                return NotFound();

            var clienteDTO = new ClienteDTOOutput(cliente.Id, cliente.Nome, cliente.Cpf, cliente.Email, cliente.Cep);
            return Ok(clienteDTO);
        }

        [HttpGet("count")]
        public async Task<ActionResult<int>> GetTotalClientes()
        {
            var total = (await _clienteService.GetAllAsync()).Count();
            return Ok(total);
        }

        [HttpPost]
        public async Task<ActionResult<ClienteDTOOutput>> CreateCliente([FromForm] ClienteDTOInput novoClienteDTO)
        {
            var clientes = await _clienteService.GetAllAsync();
            if (clientes.Any(c => c.Cpf == novoClienteDTO.cpf))
                return BadRequest("Já existe um cliente com este CPF");

            var novoCliente = new Cliente(
                novoClienteDTO.nome,
                novoClienteDTO.cpf,
                novoClienteDTO.email,
                novoClienteDTO.senha,
                novoClienteDTO.cep
            );

            await _clienteService.CriarClienteAsync(novoCliente);

            var clienteDTO = new ClienteDTOOutput(novoCliente.Id, novoCliente.Nome, novoCliente.Cpf, novoCliente.Email, novoCliente.Cep);
            return CreatedAtAction(nameof(GetClienteId), new { id = novoCliente.Id }, clienteDTO);
        }

        [HttpPost("login")]
        public async Task<ActionResult<ClienteDTOOutput>> LoginCliente([FromForm] LoginDTO loginDTO)
        {
            var clientes = await _clienteService.GetAllAsync();
            var cliente = clientes.FirstOrDefault(c => c.Email == loginDTO.email && c.Senha == loginDTO.senha);

            if (cliente == null)
                return Unauthorized("Email ou senha inválidos");

            var clienteDTO = new ClienteDTOOutput(cliente.Id, cliente.Nome, cliente.Cpf, cliente.Email, cliente.Cep);
            return Ok(clienteDTO);
        }

        [HttpPatch("{id:guid}")]
        public async Task<IActionResult> UpdateCliente(Guid id, ClienteDTOInput clienteAtualizadoDTO)
        {
            var cliente = await _clienteService.GetByIdAsync(id);
            if (cliente == null)
                return NotFound();

            var clientes = await _clienteService.GetAllAsync();
            if (clientes.Any(c => c.Id != id && c.Cpf == clienteAtualizadoDTO.cpf))
                return BadRequest("Já existe um cliente com esse CPF");

            if (!string.IsNullOrWhiteSpace(clienteAtualizadoDTO.nome)) cliente.Nome = clienteAtualizadoDTO.nome;
            if (!string.IsNullOrWhiteSpace(clienteAtualizadoDTO.cpf)) cliente.Cpf = clienteAtualizadoDTO.cpf;
            if (!string.IsNullOrWhiteSpace(clienteAtualizadoDTO.email)) cliente.Email = clienteAtualizadoDTO.email;
            if (!string.IsNullOrWhiteSpace(clienteAtualizadoDTO.senha)) cliente.Senha = clienteAtualizadoDTO.senha;
            if (!string.IsNullOrWhiteSpace(clienteAtualizadoDTO.cep)) cliente.Cep = clienteAtualizadoDTO.cep;

            await _clienteService.AtualizarClienteAsync(cliente);
            return NoContent();
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> DeleteCliente(Guid id)
        {
            await _clienteService.RemoverClienteAsync(id);
            return NoContent();
        }
    }
}
