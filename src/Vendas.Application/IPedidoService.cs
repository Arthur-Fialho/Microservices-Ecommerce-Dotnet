using Vendas.Application.DTOs;

namespace Vendas.Application
{
    public interface IPedidoService
    {
        Task<bool> CriarPedidoAsync(CriarPedidoDto criarPedidoDto);
    }
}