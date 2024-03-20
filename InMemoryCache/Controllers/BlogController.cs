using InMemoryCache.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace InMemoryCache.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BlogController : Controller
    {
        private readonly IMemoryCache _cache;

        private readonly AppDbContext _appDbContext;

        public BlogController(AppDbContext appDbContext, IMemoryCache cache)
        {
            _appDbContext = appDbContext;
            _cache = cache;
        }

        #region GetBlogs

        [HttpGet]
        public async Task<IActionResult> GetBlogs()
        {
            try
            {
                List<BlogDataModel>? blogs = new();

                if (_cache.TryGetValue("blogs", out blogs))
                {
                    return Ok(blogs);
                }

                blogs = await _appDbContext.Blogs
                    .AsNoTracking()
                    .OrderByDescending(x => x.Blog_Id)
                    .ToListAsync();

                _cache.Set("blogs", blogs, TimeSpan.FromSeconds(10));

                return (Ok(blogs));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        #endregion
    }
}
