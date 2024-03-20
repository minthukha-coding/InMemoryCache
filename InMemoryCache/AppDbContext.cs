using InMemoryCache.Models;
using Microsoft.EntityFrameworkCore;

namespace InMemoryCache;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions options) : base(options)
    {
    }
    public DbSet<BlogDataModel> Blogs { get; set; }
}