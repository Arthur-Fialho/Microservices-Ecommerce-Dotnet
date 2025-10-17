// Em Vendas.API/Controllers/PedidosController.cs
using Microsoft.AspNetCore.Mvc;
using Vendas.Application;
using Vendas.Application.DTOs;
using Microsoft.AspNetCore.Authorization;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class PedidosController : ControllerBase
{
    private readonly IPedidoService _pedidoService;

    public PedidosController(IPedidoService pedidoService)
    {
        _pedidoService = pedidoService;
    }

    [HttpPost]
    public async Task<IActionResult> CriarPedido([FromBody] CriarPedidoDto criarPedidoDto)
    {
        var sucesso = await _pedidoService.CriarPedidoAsync(criarPedidoDto);

        if (!sucesso)
        {
            // Retorna um 400 Bad Request se a validação (estoque, produto não existe) falhar.
            return BadRequest("Não foi possível criar o pedido. Verifique o estoque ou os produtos informados.");
        }

        // Retorna um 200 OK.
        return Ok("Pedido criado com sucesso.");
    }
}