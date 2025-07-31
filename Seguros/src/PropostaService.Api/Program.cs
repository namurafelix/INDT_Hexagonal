using Microsoft.EntityFrameworkCore;
using PropostaService.Application.Services;
using PropostaService.Domain.Repositories;
using PropostaService.Infrastructure.Persistence;
using PropostaService.Infrastructure.Repositories;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<PropostaDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("PropostaDb")));

builder.Services.AddScoped<IPropostaRepository, PropostaRepository>();
builder.Services.AddScoped<PropostaAppService>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "PropostaService.Api", Version = "v1" });
});

var app = builder.Build();

// Apply migrations & seed (only in Development)
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<PropostaDbContext>();
    db.Database.Migrate();

    if (app.Environment.IsDevelopment())
    {
        await DataSeeder.SeedAsync(db);
    }
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "PropostaService.Api v1");
        c.RoutePrefix = string.Empty;
    });
}

app.UseAuthorization();
app.MapControllers();
app.Run();
