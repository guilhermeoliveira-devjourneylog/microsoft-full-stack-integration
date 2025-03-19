var builder = WebApplication.CreateBuilder(args);

// Adicionar suporte a CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowBlazor",
        policy => policy.WithOrigins("https://localhost:7175")
                        .AllowAnyMethod()
                        .AllowAnyHeader());
});

builder.Services.AddControllers();
builder.Services.AddOpenApi();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

// Ativar CORS na API
app.UseCors("AllowBlazor");

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

// Mapeamento do endpoint manual
app.MapGet("/api/products", () =>
{
    return new[]
    {
        new { Id = 1, Name = "Laptop", Price = 1200.50, Stock = 25 },
        new { Id = 2, Name = "Headphones", Price = 50.00, Stock = 100 }
    };
});

app.Run();
