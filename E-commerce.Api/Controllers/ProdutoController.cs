using Dunder_Store.Database;
using Dunder_Store.DTO;
using Dunder_Store.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Dunder_Store.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProdutoController : ControllerBase
    {
        private readonly ProdutosDbContext dbContext;

        public ProdutoController(ProdutosDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Produto>>> GetProdutos()
        {
            var produtos = await dbContext.Produtos
                .OrderBy(p => p.Nome)
                .ToListAsync();

            if (!produtos.Any())
                return NoContent();

            return Ok(produtos);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Produto>> GetProdutoById(string id)
        {
            var produto = await dbContext.Produtos.FirstOrDefaultAsync(p => p.Id == id);

            if (produto is null)
                return NotFound("Produto não encontrado.");

            return Ok(produto);
        }

        [HttpGet("codigoDeBarra/{codigoDeBarra}")]
        public async Task<ActionResult<Produto>> GetProdutoCodigoDeBarra(string codigoDeBarra)
        {
            var produto = await dbContext.Produtos.FirstOrDefaultAsync(p => p.CodigoDeBarra == codigoDeBarra);

            if (produto is null)
                return NotFound("Produto não encontrado com este código de barra.");

            return Ok(produto);
        }

        [HttpGet("nome/{nomeProduto}")]
        public async Task<ActionResult<Produto>> GetProdutoNome(string nomeProduto)
        {
            var produto = await dbContext.Produtos.FirstOrDefaultAsync(p => p.Nome == nomeProduto);

            if (produto is null)
                return NotFound("Produto não encontrado com este nome.");

            return Ok(produto);
        }

        [Authorize] // Exige Token Administrativo
        [HttpPost]
        public async Task<ActionResult<Produto>> CreateProduto([FromForm] ProdutoDTO novoProdutoDTO, IFormFile imagem)
        {
            try
            {
                if (await dbContext.Produtos.AnyAsync(produto => produto.CodigoDeBarra == novoProdutoDTO.codigoDeBarra))
                    return BadRequest("Já existe um produto com este código de barras.");

                if (imagem == null || imagem.Length == 0)
                    return BadRequest("Imagem obrigatória.");

                string extensaoArquivo = Path.GetExtension(imagem.FileName);
                string nomePasta = "produtos";
                string caminhoDaPasta = Path.Combine("wwwroot", nomePasta);
                Directory.CreateDirectory(caminhoDaPasta);

                string nomeArquivo = $"{Guid.NewGuid()}{extensaoArquivo}";
                string caminhoCompleto = Path.Combine(caminhoDaPasta, nomeArquivo);

                using (var stream = new FileStream(caminhoCompleto, FileMode.Create))
                {
                    await imagem.CopyToAsync(stream);
                }

                string urlServidor = $"{Request.Scheme}://{Request.Host}";
                string imagemUrl = $"{urlServidor}/{nomePasta}/{nomeArquivo}";

                var novoProduto = new Produto(
                    novoProdutoDTO.nome,
                    novoProdutoDTO.descricao,
                    novoProdutoDTO.preco,
                    novoProdutoDTO.codigoDeBarra,
                    imagemUrl
                );

                await dbContext.Produtos.AddAsync(novoProduto);
                await dbContext.SaveChangesAsync();

                return CreatedAtAction(nameof(GetProdutoById), new { id = novoProduto.Id }, novoProduto);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro ao criar produto: {ex.Message}");
            }
        }

        [Authorize] // Exige Token Administrativo
        [HttpPatch("{id}")]
        public async Task<IActionResult> UpdateProduto(string id, ProdutoDTO produtoAtualizadoDTO)
        {
            var produto = await dbContext.Produtos.FirstOrDefaultAsync(p => p.Id == id);

            if (produto is null)
                return NotFound("Produto não encontrado.");

            if (await dbContext.Produtos.AnyAsync(p => p.CodigoDeBarra == produtoAtualizadoDTO.codigoDeBarra && p.Id != id))
                return BadRequest("Já existe outro produto com este código de barras.");

            if (!string.IsNullOrWhiteSpace(produtoAtualizadoDTO.nome) && produtoAtualizadoDTO.nome != "string")
                produto.Nome = produtoAtualizadoDTO.nome;

            if (!string.IsNullOrWhiteSpace(produtoAtualizadoDTO.descricao) && produtoAtualizadoDTO.descricao != "string")
                produto.Descricao = produtoAtualizadoDTO.descricao;

            if (produtoAtualizadoDTO.preco > 0)
                produto.Preco = produtoAtualizadoDTO.preco;

            if (!string.IsNullOrWhiteSpace(produtoAtualizadoDTO.codigoDeBarra) && produtoAtualizadoDTO.codigoDeBarra != "string")
                produto.CodigoDeBarra = produtoAtualizadoDTO.codigoDeBarra;

            await dbContext.SaveChangesAsync();

            return NoContent();
        }

        [Authorize] // Exige Token Administrativo
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduto(string id)
        {
            var produto = await dbContext.Produtos
                .Include(p => p.PedidoProdutos)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (produto == null)
                return NotFound("Produto não encontrado.");

            dbContext.PedidoProdutos.RemoveRange(produto.PedidoProdutos);
            dbContext.Produtos.Remove(produto);
            await dbContext.SaveChangesAsync();

            return NoContent();
        }
    }
}
