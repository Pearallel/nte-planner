using Microsoft.EntityFrameworkCore;
using nte_planner.Models;
using Microsoft.JSInterop; // ADD THIS
using System.IO;         // ADD THIS
namespace nte_planner.Data;

public class AppDbContext : DbContext
{

    private readonly IJSRuntime _jsRuntime;

    public AppDbContext(DbContextOptions<AppDbContext> options, IJSRuntime jsRuntime) : base(options)
    {
        _jsRuntime = jsRuntime;
    }

    // Core Lookup Tables
    public DbSet<Rarity> Rarities { get; set; }
    public DbSet<CombatRole> CombatRoles { get; set; }
    public DbSet<Element> Elements { get; set; }
    public DbSet<ArcCompatability> ArcCompatabilities { get; set; }
    public DbSet<HunterLevel> HunterLevels { get; set; }

    // Primary Entities
    public DbSet<Esper> Espers { get; set; }
    public DbSet<Arc> Arcs { get; set; }
    public DbSet<Cartridge> Cartridges { get; set; }
    public DbSet<Module> Modules { get; set; }
    public DbSet<Item> Items { get; set; }
    public DbSet<Profile> Profiles { get; set; }
    public DbSet<FarmOption> FarmOptions { get; set; }
    public DbSet<UpgradeRecipe> UpgradeRecipes { get; set; }

    // Junction / Mapping Tables (These require the composite keys)
    public DbSet<OwnedEsper> OwnedEspers { get; set; }
    public DbSet<OwnedArc> OwnedArcs { get; set; }
    public DbSet<OwnedCartridge> OwnedCartridges { get; set; }
    public DbSet<OwnedModule> OwnedModules { get; set; }
    public DbSet<OwnedItem> OwnedItems { get; set; }
    public DbSet<FarmOptionAvgReward> FarmOptionAvgRewards { get; set; }
    public DbSet<UpgradeRecipeItem> UpgradeRecipeItems { get; set; }

    // Add this method to AppDbContext:
    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        base.ConfigureConventions(configurationBuilder);

        // This tells EF Core's discovery engine to completely ignore these types globally 
        // before it even attempts to map them to SQLite.
        configurationBuilder.IgnoreAny<Dictionary<string, string>>();
        configurationBuilder.IgnoreAny<Supabase.Postgrest.ClientOptions>();
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // 1. Ignore the BaseModel type itself
        modelBuilder.Ignore<Supabase.Postgrest.Models.BaseModel>();

        // 2. We can still use Reflection here to clean up any remaining simple inherited properties
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            if (typeof(Supabase.Postgrest.Models.BaseModel).IsAssignableFrom(entityType.ClrType))
            {
                var baseProps = typeof(Supabase.Postgrest.Models.BaseModel).GetProperties();
                foreach (var prop in baseProps)
                {
                    modelBuilder.Entity(entityType.ClrType).Ignore(prop.Name);
                }
            }
        }

        // 3. Configure Composite Primary Keys for "Owned" entities
        modelBuilder.Entity<OwnedEsper>().HasKey(oe => new { oe.EsperId, oe.ProfileId });
        modelBuilder.Entity<OwnedArc>().HasKey(oa => new { oa.ArcId, oa.ProfileId });
        modelBuilder.Entity<OwnedCartridge>().HasKey(oc => new { oc.CartridgeId, oc.ProfileId });
        modelBuilder.Entity<OwnedModule>().HasKey(om => new { om.ModuleId, om.ProfileId });
        modelBuilder.Entity<OwnedItem>().HasKey(oi => new { oi.ItemId, oi.ProfileId });

        // 4. Configure Composite Primary Keys for other junction tables
        modelBuilder.Entity<FarmOptionAvgReward>().HasKey(fr => new { fr.FarmOptionId, fr.ItemId });
        modelBuilder.Entity<UpgradeRecipeItem>().HasKey(uri => new { uri.UpgradeRecipeId, uri.ItemId });
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        // 1. Let EF Core save to the in-memory SQLite database
        int result = await base.SaveChangesAsync(cancellationToken);

        // 2. Force SQLite to flush the WAL file into the main database file instantly
        await Database.ExecuteSqlRawAsync("PRAGMA wal_checkpoint(TRUNCATE);", cancellationToken);

        // 3. Immediately backup the fully updated file bytes to the browser's IndexedDB
        try
        {
            var dbBytes = File.ReadAllBytes("nteplanner_local.db");
            await _jsRuntime.InvokeVoidAsync("indexedDbInterop.save", "nteplanner_local.db", dbBytes);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to sync to IndexedDB: {ex.Message}");
        }

        return result;
    }
}