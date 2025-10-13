using Microsoft.AspNetCore.Builder;
using Vendas.Domain;
using Vendas.Infrastructure;
using Swashbuckle.AspNetCore.SwaggerUI;

var builder = WebApplication.CreateBuilder(args);

// Adicionar o serviço do HttpClient tipado
builder.Services.AddHttpClient<IEstoqueServiceHttpClient, EstoqueServiceHttpClient>(client =>
{
    // A URL base do microserviço de Estoque.
    client.BaseAddress = new Uri(builder.Configuration["ServicesUrl:EstoqueApi"]);
});

// Add services to the container.

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