using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplicationPR4.Models; 
using System.Linq;

namespace WebApplicationPR4.Controlers 
{
    [ApiController]
    [Route("api/[controller]")] 
    public class RController : ControllerBase
    {
        private readonly PR4Context _context;

        public RController(PR4Context context)
        {
            _context = context;
        }

        [HttpGet("user-reviews/{userId}")]
        public async Task<ActionResult<List<Review>>> GetUserReviews(int userId)
        {

            var reviewsRaw = await _context.Reviews
                .Where(r => r.Iduser == userId)
                .Select(r => new { r.Id, r.Iduser, r.Idproduct, r.Rating, r.Description })
                .ToListAsync();

            if (!reviewsRaw.Any())
                return Ok(new List<Review>());

            var userIds = reviewsRaw.Select(r => r.Iduser).Distinct().ToList();
            var productIds = reviewsRaw.Select(r => r.Idproduct).Distinct().ToList();

            var usersDict = await _context.Users
                .Where(u => userIds.Contains(u.Id))
                .ToDictionaryAsync(u => u.Id, u => u.Name);

            var productsDict = await _context.Products
                .Where(p => productIds.Contains(p.Id))
                .ToDictionaryAsync(p => p.Id, p => p.Name);

            var result = reviewsRaw.Select(r => new Review
            {
                Id = r.Id,

                UserName = usersDict.TryGetValue(r.Iduser, out var uName) ? uName : "Неизвестно",

                ProductName = productsDict.TryGetValue(r.Idproduct, out var pName) ? pName : "Без названия",

                Rating = r.Rating,
                Description = r.Description,
            }).ToList();

            return Ok(result);
        }
    }
}