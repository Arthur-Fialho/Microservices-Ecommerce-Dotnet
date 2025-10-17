var builder = WebApplication.CreateBuilder(args);

// 1. Carrega a configuração do YARP do appsettings.json
builder.Services.AddReverseProxy()
    .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));

builder.Services.AddControllers();

var app = builder.Build();

// 2. Registra o pipeline do YARP que irá lidar com o roteamento
app.MapReverseProxy();

app.MapControllers();

app.Run();