using HPlusSport.API.Models;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers()
    .ConfigureApiBehaviorOptions(options =>
    {
        // Deaktiverer automatisk 400-fejl fra [ApiController]. 
        // Gør det muligt selv at styre valideringslogikken i controlleren.
        options.SuppressModelStateInvalidFilter = true;
    });

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<ShopContext>(options =>
    options.UseInMemoryDatabase("ShopDb"));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ShopContext>();
        await db.Database.EnsureCreatedAsync();
}

    app.MapGet("/Products", async (ShopContext _context) =>
    {
        return await _context.Products.ToArrayAsync();
    });

// Det første endpoint er faktisk korrekt, hvis _context er stavet rigtigt
app.MapGet("/products/{id}", async (int id, ShopContext _context) =>
{
    var product = await _context.Products.FindAsync(id);
    if (product == null)
    {
        return Results.NotFound();
    }
    return Results.Ok(product);
});

// Det andet endpoint rettet for stavefejl og syntaks
app.MapGet("/products/available", async (ShopContext _context) =>
{
    var products = await _context.Products.Where(p => p.IsAvailable).ToArrayAsync();
    return Results.Ok(products);
});

app.Run();
