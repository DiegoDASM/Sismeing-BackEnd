using Microsoft.EntityFrameworkCore;
using Sismeing.Infrestructura.Persistence;

var builder = WebApplication.CreateBuilder(args);

var ConnectionStrings = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

builder.Services.AddDbContext<SupaBaseDBcontext>(options =>
    options.UseNpgsql(ConnectionStrings));

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
