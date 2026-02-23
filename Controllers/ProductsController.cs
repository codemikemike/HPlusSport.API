using HPlusSport.API.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HPlusSport.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly ShopContext _context;

        public ProductsController(ShopContext context)
        {
            _context = context;

            // Sikrer at In-Memory databasen er oprettet og Seed-data er indlæst
            _context.Database.EnsureCreated();
        }

        // Henter alle produkter: GET api/products
        [HttpGet]
        public async Task<ActionResult> GetAllProducts()
        {
            return Ok(await _context.Products.ToArrayAsync());
        }

        // Henter et specifikt produkt: GET api/products/1
        // Bemærk {id} med krølleparenteser - det gør det til en variabel
        [HttpGet("{id}")]
        public async Task<ActionResult> GetProductById(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            return Ok(product);
        }

        // Henter tilgængelige produkter: GET api/products/available
        [HttpGet("available")]
        public async Task<ActionResult> GetAvailableProducts()
        {
            var products = await _context.Products.Where(p => p.IsAvailable).ToArrayAsync();
            return Ok(products);
        }

        // Opretter et nyt produkt: POST api/products
        [HttpPost]
        public async Task<ActionResult> PostProduct(Product product)
        {

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.Products.Add(product);
            await _context.SaveChangesAsync();

            return CreatedAtAction(
                nameof(GetProductById), // Navnet på metoden ovenfor
                new { id = product.Id }, // ID'et på det nye produkt
                product);
        }

        [HttpPut("{id}")] // Rettet: Ruten skal stå inde i parentesen
        public async Task<ActionResult> PutProduct(int id, Product product)
        {
            if (id != product.Id)
            {
                return BadRequest("ID i URL og body skal være ens.");
            }

            _context.Entry(product).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Products.Any(p => p.Id == id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            return NoContent();
        }

            [HttpDelete("{id}")] // Rettet: Ruten skal stå inde i parentesen
            public async Task<ActionResult> DeleteProduct(int id)
            {
                var product = await _context.Products.FindAsync(id);
                if (product == null)
                {
                    return NotFound();
                }
                _context.Products.Remove(product);
                await _context.SaveChangesAsync();
                return Ok(product);
        }

    } // Closes the Class
} // Closes the Namespace