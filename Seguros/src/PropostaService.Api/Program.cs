using Microsoft.EntityFrameworkCore;
using PropostaService.Application.Services;
using PropostaService.Domain.Repositories;
using PropostaService.Infrastructure.Persistence;
using PropostaService.Infrastructure.Repositories;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<PropostaDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("PropostaDb")));

builder.Services.AddScoped<IPropostaRepository, PropostaRepository>();
builder.Services.AddScoped<PropostaAppService>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

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
    app.UseSwaggerUI();
}

app.UseAuthorization();
app.MapControllers();
app.Run();
