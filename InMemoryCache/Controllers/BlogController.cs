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

        #region UpdateBlog

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateBlog(int id, BlogDataModel model)
        {
            try
            {
                var item = await _appDbContext.Blogs.FirstOrDefaultAsync(x => x.Blog_Id == id);
                if (item == null)
                {
                    return NotFound("No data found");
                }

                item.Blog_Title = model.Blog_Title;
                item.Blog_Author = model.Blog_Author;
                item.Blog_Content = model.Blog_Content;

                _appDbContext.Attach(item);

                int result = await _appDbContext.SaveChangesAsync();

                if (result > 0)
                {
                    List<BlogDataModel> updatedBlogs = await _appDbContext.Blogs
                        .OrderByDescending(x => x.Blog_Id)
                        .ToListAsync();

                    _cache.Set("blogs", updatedBlogs);

                    return Ok(updatedBlogs); // Return a success response
                }
                else
                {
                    return BadRequest("Failed to update the blog entry."); // Return a failure response
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        #endregion
    }
}
