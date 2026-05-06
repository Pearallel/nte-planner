using Microsoft.EntityFrameworkCore;

namespace nte_planner.Models;

public class LocalDbContext : DbContext
{
    public DbSet<Todo> Todos { get; set; }

    public LocalDbContext(DbContextOptions<LocalDbContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Todo>(entity =>
        {
            // Using string names bypasses the compile-time checks for hidden 
            // Supabase properties, but still tells EF Core to ignore them at runtime.
            entity.Ignore("ClientOptions");
            entity.Ignore("RequestClientOptions");
            entity.Ignore("BaseUrl");
            entity.Ignore("Endpoint");
            entity.Ignore("TableName");
            entity.Ignore("PrimaryKeyValue");
        });
    }
}