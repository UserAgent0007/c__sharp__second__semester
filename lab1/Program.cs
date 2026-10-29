using DotNetEnv;
using lab1.Components;
using lab1.Models;
using lab1.Services;
using Microsoft.EntityFrameworkCore;

Env.Load();

var builder = WebApplication.CreateBuilder(args);

var connectionString = Environment.GetEnvironmentVariable("DATABASEURL")
    ?? throw new InvalidOperationException("Connection string 'DATABASEURL' not found in .env file.");

// Контекст для Web API-контролерів (на кожен HTTP-запит свій екземпляр)
builder.Services.AddScoped(sp => new ProjectContractsContext(connectionString));

// Фабрика контекстів для Blazor-компонентів/сервісів
builder.Services.AddSingleton<IDbContextFactory<ProjectContractsContext>>(
    new ProjectContractsContextFactory(connectionString));

// Сервіси, які використовують Blazor-компоненти
builder.Services.AddScoped<EmployeeService>();
builder.Services.AddScoped<PositionService>();
builder.Services.AddScoped<ProjectService>();
builder.Services.AddScoped<EmployeeProjectService>();

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
    });

// Blazor Server (замість AddRazorPages)
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.UseAuthorization();
app.UseAntiforgery();   // обов'язково для Blazor Web App

app.MapStaticAssets();
app.MapControllers();   // Web API (маршрути api/[controller])
app.MapRazorComponents<App>()   // Blazor UI (замість MapRazorPages)
    .AddInteractiveServerRenderMode();

app.Run();
