using DotNetEnv;
using lab1.Models;
using Microsoft.EntityFrameworkCore;

Env.Load();

var builder = WebApplication.CreateBuilder(args);

var connectionString = Environment.GetEnvironmentVariable("DATABASEURL")
    ?? throw new InvalidOperationException("Connection string 'DATABASEURL' not found in .env file.");

builder.Services.AddScoped(sp => new ProjectContractsContext(connectionString));

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
    });

builder.Services.AddRazorPages();

builder.Services.AddHttpClient("MyApi", (sp, client) =>
{
    var httpContextAccessor = sp.GetRequiredService<IHttpContextAccessor>();
    var request = httpContextAccessor.HttpContext?.Request;
    if (request != null)
    {
        // Автоматично бере поточний базовий URL додатка (протокол + хост + порт)
        client.BaseAddress = new Uri($"{request.Scheme}://{request.Host}/");
    }
});

builder.Services.AddHttpContextAccessor();

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

app.MapControllers(); // Для API-контролерів (маршрути api/[controller])
app.MapRazorPages();  // Для Razor Pages (UI)

app.Run();