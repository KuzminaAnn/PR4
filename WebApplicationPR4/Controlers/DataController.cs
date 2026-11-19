using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplicationPR4.Models;

namespace WebApplicationPR4.Controlers
{
    [ApiController]
    [Route("api/products")]
    public class DataController : ControllerBase
    {
        private readonly PR4Context _context;
        public DataController(PR4Context context)
        {
            _context = context;
        }

        // GET: api/data/getitems
        [HttpGet("getitems")]
        public async Task<ActionResult<List<Product>>> GetItems()
        {
            var products = await _context.Products.Where(p => p.Quantity > 0 && !p.IsOver).ToListAsync();
            return Ok(products);
        }
    }
}
