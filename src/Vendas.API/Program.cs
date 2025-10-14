using Vendas.Domain;
using Vendas.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Vendas.Application;
using Vendas.Application.DTOs;

var builder = WebApplication.CreateBuilder(args);

// Configuração do DbContext de Vendas
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<VendasDbContext>(options =>
    options.UseSqlServer(connectionString));

// Configuração do HttpClient para o serviço de Estoque
builder.Services.AddHttpClient<IEstoqueServiceHttpClient, EstoqueServiceHttpClient>(client =>
{
    client.BaseAddress = new Uri(builder.Configuration["ServicesUrl:EstoqueApi"]);
});

// Registro das dependências (Interface -> Implementação)
builder.Services.AddScoped<IPedidoRepository, PedidoRepository>();
builder.Services.AddScoped<IPedidoService, PedidoService>();

builder.Services.AddControllers();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();