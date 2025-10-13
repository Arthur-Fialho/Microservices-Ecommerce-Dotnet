using System.Net.Http.Json;
using Vendas.Domain;
using Vendas.Domain.DTOs;

namespace Vendas.Infrastructure
{
    public class EstoqueServiceHttpClient : IEstoqueServiceHttpClient
    {
        private readonly HttpClient _httpClient;

        public EstoqueServiceHttpClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<ProdutoDto?> GetProdutoByIdAsync(Guid produtoId)
        {
            var response = await _httpClient.GetAsync($"/api/produtos/{produtoId}");
            
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<ProdutoDto>();
            }

            return null;
        }
    }
}