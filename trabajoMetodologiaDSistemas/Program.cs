using Microsoft.EntityFrameworkCore;
using trabajoMetodologiaDSistemas.Data;
using System;
using Microsoft.Extensions.Logging;

var builder = WebApplication.CreateBuilder(args);
// Añadir consola para logs
builder.Logging.AddConsole();

builder.Services.AddControllersWithViews();
builder.Services.AddSession();

// Usar base de datos en memoria para desarrollo/front sin SQL Server
//builder.Services.AddDbContext<AppDbContext>(options =>
//    options.UseInMemoryDatabase("TestDb"));

// Obtiene la cadena de conexión desde appsettings.json
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

// Configura el DbContext para usar SQL Server
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(connectionString));

var app = builder.Build();

// Creacion de base de datos y tablas si no existen
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.EnsureCreated();
}

// No se realiza verificación de conexión ni creación de esquema aquí
// La base en memoria se crea automáticamente al acceder

if (!app.Environment.IsDevelopment())
{
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthorization();
app.UseSession();
app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.Run();
