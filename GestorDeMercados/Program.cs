using Microsoft.EntityFrameworkCore;
using GestorDeMercados.Data;
using GestorDeMercados.Repositories.DAO;
using GestorDeMercados.Repositories.Interfaces;
using GestorDeMercados.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddDbContext<APPDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("cadena")));
builder.Services.AddScoped<IRol, rolDAO>();
builder.Services.AddScoped<IUsuario, usuarioDAO>();
builder.Services.AddScoped<IDueno, duenoDAO>();
builder.Services.AddScoped<ITienda, tiendaDAO>();
builder.Services.AddScoped<IDeuda, deudaDAO>();
builder.Services.AddScoped<IServicio, servicioDAO>();
builder.Services.AddScoped<IPago, pagoDAO>();
builder.Services.AddScoped<DeudaService>();
builder.Services.AddHostedService<DeudaBackgroundService>();
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
