using Microsoft.EntityFrameworkCore;
using nte_planner.Data;
using nte_planner.Models;

namespace nte_planner.Services;

public class EsperDataService
{
    private readonly IDbContextFactory<AppDbContext> _dbFactory;

    // Inject the DbContextFactory so we can spin up a quick, short-lived 
    // database connection exactly when we need it.
    public EsperDataService(IDbContextFactory<AppDbContext> dbFactory)
    {
        _dbFactory = dbFactory;
    }

    /// <summary>
    /// Retrieves all active Espers owned by a specific profile, 
    /// fully populated with all their related lookup data.
    /// </summary>
    /// <param name="profileId">The user's profile ID</param>
    /// <returns>A fully nested list of OwnedEspers</returns>
    public async Task<List<OwnedEsper>> GetUserEspersAsync(string profileId)
    {
        // Always use a 'using' statement with DbContext in Blazor WASM 
        // to ensure it is disposed of properly after the query runs.
        using var dbContext = await _dbFactory.CreateDbContextAsync();

        // We chain .Include() and .ThenInclude() to tell SQLite:
        // "When you grab the OwnedEsper, grab the base Esper too. 
        // And while you're at it, grab its Rarity, Role, Element, and Arc Compatibility."
        return await dbContext.OwnedEspers
            .Where(oe => oe.ProfileId == profileId && !oe.Deleted) // Filter for the active user
            .Include(oe => oe.Esper)
                .ThenInclude(e => e.Rarity)
            .Include(oe => oe.Esper)
                .ThenInclude(e => e.CombatRole)
            .Include(oe => oe.Esper)
                .ThenInclude(e => e.Element)
            .Include(oe => oe.Esper)
                .ThenInclude(e => e.ArcCompatability)
            // AsNoTracking() makes the query slightly faster because EF Core 
            // doesn't need to monitor these specific objects for update changes 
            // just for displaying them on screen.
            .AsNoTracking()
            .ToListAsync();
    }

    /// <summary>
    /// Optional: A method to get a single Esper by its ID if the user clicks into a "Details" page.
    /// </summary>
    public async Task<OwnedEsper?> GetUserEsperByIdAsync(string profileId, int esperId)
    {
        using var dbContext = await _dbFactory.CreateDbContextAsync();

        return await dbContext.OwnedEspers
            .Where(oe => oe.ProfileId == profileId && oe.EsperId == esperId && !oe.Deleted)
            .Include(oe => oe.Esper)
                .ThenInclude(e => e.Rarity)
            .Include(oe => oe.Esper)
                .ThenInclude(e => e.CombatRole)
            .Include(oe => oe.Esper)
                .ThenInclude(e => e.Element)
            .Include(oe => oe.Esper)
                .ThenInclude(e => e.ArcCompatability)
            .AsNoTracking()
            .FirstOrDefaultAsync();
    }
}