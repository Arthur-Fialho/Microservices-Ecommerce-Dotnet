using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Vendas.Domain;
using Vendas.Domain.DTOs;
using Microsoft.AspNetCore.Http;
using System.Net.Http.Headers; 

public class EstoqueServiceHttpClient : IEstoqueServiceHttpClient
{
    private readonly HttpClient _httpClient;
    private readonly IHttpContextAccessor _httpContextAccessor;

    // Injeta o IHttpContextAccessor
    public EstoqueServiceHttpClient(HttpClient httpClient, IHttpContextAccessor httpContextAccessor)
    {
        _httpClient = httpClient;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<ProdutoDto> GetProdutoByIdAsync(Guid produtoId)
    {
        var token = _httpContextAccessor.HttpContext?
                                        .Request
                                        .Headers["Authorization"]
                                        .ToString();

        if (!string.IsNullOrEmpty(token))
        {
            _httpClient.DefaultRequestHeaders.Authorization = 
                new AuthenticationHeaderValue(token.Split(" ")[0], token.Split(" ")[1]);
        }

        // A URL agora é relativa ao Gateway. O YARP vai rotear "api/produtos/{id}"
        var response = await _httpClient.GetAsync($"api/produtos/{produtoId}");

        if (response.IsSuccessStatusCode)
        {
            return await response.Content.ReadFromJsonAsync<ProdutoDto>();
        }

        // Se a resposta for 401, 404, etc., retornará null.
        return null;
    }
}