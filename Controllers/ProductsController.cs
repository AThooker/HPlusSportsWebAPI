using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HPlusSport.Classes;
using HPlusSport.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HPlusSport.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly ShopContext _ctx;
        public ProductsController(ShopContext ctx)
        {
            _ctx = ctx;
            _ctx.Database.EnsureCreated();
        }

        //Get all products from DB
        [HttpGet]
        public async Task<IActionResult> GetAllProducts([FromQuery] ProductQueryParameters queryParameters)
        {
            IQueryable<Product> products = _ctx.Products;
            if (queryParameters.MinPrice != null && queryParameters.MaxPrice != null)
            {
                products = products
                .Where(p => p.Price >= queryParameters.MinPrice.Value && p.Price <= queryParameters.MaxPrice.Value);
            }
            if (!string.IsNullOrEmpty(queryParameters.SearchTerm))
            {
                products = products
                .Where(p =>
                p.Sku.ToLower().Contains(queryParameters.SearchTerm)
                ||
                p.Name.ToLower().Contains(queryParameters.SearchTerm));
            }
            if (!string.IsNullOrEmpty(queryParameters.Sku))
            {
                products = products.Where(p => p.Sku == queryParameters.Sku);
            }
            if (!string.IsNullOrEmpty(queryParameters.Name))
            {
                products = products.Where(p => p.Name.ToLower().Contains(queryParameters.Name));
            }
            if (!string.IsNullOrEmpty(queryParameters.SortBy))
            {
                if (typeof(Product).GetProperty(queryParameters.SortBy) != null)
                {
                    products = products.OrderByCustom(queryParameters.SortBy, queryParameters.SortOrder);
                }
            }
            products = products
            .Skip(queryParameters.Size * (queryParameters.Page - 1))
            .Take(queryParameters.Size);
            return Ok(await products.ToListAsync());
        }
        //Get product by productID
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetProduct(int id)
        {
            var product = await _ctx.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            return Ok(product);
        }
        // public async Task<IActionResult> GetAllProductsAsync()
        // {
        //     var products = await _ctx.Products.ToListAsync();
        //     return Ok(products);
        // }

        [HttpPost]
        public async Task<IActionResult> PostProduct(Product model)
        {
            if (!ModelState.IsValid) return BadRequest();
            _ctx.Products.Add(model);
            await _ctx.SaveChangesAsync();
            return CreatedAtAction("GetProduct", new { id = model.Id }, model);
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> PutProduct(int id, [FromBody] Product product)
        {
            if (id != product.Id) return BadRequest();
            _ctx.Entry(product).State = EntityState.Modified;
            try
            {
                await _ctx.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (_ctx.Products.Find(id) == null)
                {
                    return NotFound();
                }
                throw;
            }
            return NoContent();
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            var product = await _ctx.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            _ctx.Products.Remove(product);
            await _ctx.SaveChangesAsync();
            return Ok(product);
        }
        [HttpDelete]
        [Route("Delete")]
        public async Task<IActionResult> DeleteProducts([FromQuery] int[] ids)
        {
            var products = new List<Product>();
            foreach (var id in ids)
            {
                var product = await _ctx.Products.FindAsync(id);
                if (product == null)
                {
                    return NotFound();
                }
                products.Add(product);
            }
            _ctx.Products.RemoveRange(products);
            await _ctx.SaveChangesAsync();
            return Ok(products);
        }

    }
}