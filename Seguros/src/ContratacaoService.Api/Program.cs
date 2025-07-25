using Microsoft.EntityFrameworkCore;
using ContratacaoService.Application.Services;
using ContratacaoService.Domain.Repositories;
using ContratacaoService.Infrastructure.Persistence;
using ContratacaoService.Infrastructure.Repositories;
using ContratacaoService.Infrastructure.Clients;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<ContratacaoDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("ContratacaoDb")));

builder.Services.AddScoped<IContratacaoRepository, ContratacaoRepository>();
builder.Services.AddScoped<ContratacaoAppService>();

builder.Services.AddHttpClient<IPropostaServiceClient, PropostaServiceClient>(client =>
{
    var baseUrl = builder.Configuration.GetValue<string>("PropostaService:BaseUrl");
    ArgumentNullException.ThrowIfNullOrEmpty(baseUrl, nameof(baseUrl));
    client.BaseAddress = new Uri(baseUrl);
});

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Apply migrations
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ContratacaoDbContext>();
    db.Database.Migrate();
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();
app.MapControllers();
app.Run();
