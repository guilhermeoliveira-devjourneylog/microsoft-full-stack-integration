using Microsoft.Extensions.Caching.Memory;

var builder = WebApplication.CreateBuilder(args);

// Enable CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowBlazor",
        policy => policy.WithOrigins("https://localhost:7175")
                        .AllowAnyMethod()
                        .AllowAnyHeader());
});

// Implement in-memory caching for product list
builder.Services.AddMemoryCache();

builder.Services.AddControllers();
builder.Services.AddOpenApi();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

// Enable CORS
app.UseCors("AllowBlazor");

app.UseHttpsRedirection();
app.UseAuthorization();

app.MapControllers();

// Cache and return product data
app.MapGet("/api/productlist", async (IMemoryCache cache) =>
{
    const string cacheKey = "product_list";

    if (!cache.TryGetValue(cacheKey, out object products))
    {
        products = new[]
        {
            new { Id = 1, Name = "Laptop", Price = 1200.50, Stock = 25, Category = new { Id = 101, Name = "Electronics" } },
            new { Id = 2, Name = "Headphones", Price = 50.00, Stock = 100, Category = new { Id = 102, Name = "Accessories" } }
        };

        var cacheOptions = new MemoryCacheEntryOptions()
            .SetAbsoluteExpiration(TimeSpan.FromMinutes(10));

        cache.Set(cacheKey, products, cacheOptions);
    }

    return Results.Json(products);
});

app.Run();
