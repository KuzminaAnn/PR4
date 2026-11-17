using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplicationPR4.Models;

namespace WebApplicationPR4.Controlers
{
    [ApiController]
    [Route("api/[controller]")]
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
            return await _context.Products.ToListAsync();
        }
    }
}
