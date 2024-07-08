using Microsoft.EntityFrameworkCore;
using SliceWebApp.API.Entities;

namespace SliceWebApp.API.Database;

public sealed class DataContext : DbContext
{
    public DataContext(DbContextOptions<DataContext> options)
        : base(options)
    {
    }

    public DbSet<Post> Posts { get; set; }

}
