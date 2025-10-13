using Estoque.Application;
using Estoque.Application.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace Estoque.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProdutosController : ControllerBase
    {
        private readonly IProdutoService _produtoService;

        public ProdutosController(IProdutoService produtoService)
        {
            _produtoService = produtoService;
        }

        // POST: api/Produtos
        [HttpPost]
        public async Task<ActionResult<ProdutoDto>> CriarProduto([FromBody] CriarProdutoDto criarProdutoDto)
        {
            var produtoDto = await _produtoService.CriarProdutoAsync(criarProdutoDto);
            return CreatedAtAction(nameof(ObterProdutoPorId), new { id = produtoDto.Id }, produtoDto);
        }

        // GET: api/Produtos/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<ProdutoDto>> ObterProdutoPorId(Guid id)
        {
            var produtoDto = await _produtoService.ObterProdutoPorIdAsync(id);
            if (produtoDto == null) return NotFound();

            return Ok(produtoDto);
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProdutoDto>>> ObterTodosProdutos()
        {
            var produtos = await _produtoService.ObterTodosProdutosAsync();
            return Ok(produtos);
        }
    }
}