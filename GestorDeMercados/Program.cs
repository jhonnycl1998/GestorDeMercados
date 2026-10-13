using Microsoft.EntityFrameworkCore;
using GestorDeMercados.Data;
using GestorDeMercados.Repositories.DAO;
using GestorDeMercados.Repositories.Interfaces;
using GestorDeMercados.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

//autenticacion

var jwt = builder.Configuration.GetSection("Jwt");

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(
    o =>
    {
        o.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwt["Issuer"],
            ValidAudience = jwt["Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt["Key"]!))
        };
    });

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

using(var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<APPDbContext>();
    db.Database.Migrate();
    DbSeeder.Seed(db, builder.Configuration);
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
