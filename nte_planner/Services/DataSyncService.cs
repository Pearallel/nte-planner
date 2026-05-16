using Microsoft.EntityFrameworkCore;
using Microsoft.JSInterop;
using nte_planner.Data;
using nte_planner.Models;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace nte_planner.Services;

public class DataSyncService
{
    private readonly IDbContextFactory<AppDbContext> _dbFactory;
    private readonly Supabase.Client _supabase;
    private readonly IJSRuntime _jsRuntime;

    public DataSyncService(
        IDbContextFactory<AppDbContext> dbFactory,
        Supabase.Client supabase,
        IJSRuntime jsRuntime)
    {
        _dbFactory = dbFactory;
        _supabase = supabase;
        _jsRuntime = jsRuntime;
    }

    /// <summary>
    /// Master method to run the full sync process.
    /// </summary>
    public async Task RunFullSyncAsync()
    {
        try
        {
            await SyncBaseStaticDataAsync();
            await PushLocalChangesToSupabaseAsync();
            await PullRemoteChangesFromSupabaseAsync();
            await SaveDatabaseToBrowserAsync();
        }
        catch (Exception ex)
        {
            // In a production app, log this. It likely means the user is offline,
            // which is fine! The local DB will just sync next time.
            Console.WriteLine($"Sync failed (likely offline): {ex.Message}");
        }
    }

    private async Task PushLocalChangesToSupabaseAsync()
    {
        using var dbContext = await _dbFactory.CreateDbContextAsync();

        // 1. Find all local records that were created/modified offline
        var dirtyEspers = await dbContext.OwnedEspers
            .Where(oe => oe.IsLocalDirty)
            .ToListAsync();

        if (!dirtyEspers.Any()) return;

        // 2. Push them to Supabase using Upsert (Insert or Update)
        var response = await _supabase.From<OwnedEsper>().Upsert(dirtyEspers);

        if (response.Models.Any())
        {
            // 3. If successful, mark them as clean locally
            foreach (var esper in dirtyEspers)
            {
                esper.IsLocalDirty = false;
            }

            dbContext.OwnedEspers.UpdateRange(dirtyEspers);
            await dbContext.SaveChangesAsync();
        }
    }

    private async Task PullRemoteChangesFromSupabaseAsync()
    {
        using var dbContext = await _dbFactory.CreateDbContextAsync();

        // 1. Get the timestamp of the most recently updated local record
        var lastLocalUpdate = await dbContext.OwnedEspers
            .MaxAsync(oe => oe.LastUpdatedAt) ?? DateTime.MinValue;

        // 2. Ask Supabase for anything newer than our local timestamp
        var response = await _supabase.From<OwnedEsper>()
            .Where(oe => oe.LastUpdatedAt > lastLocalUpdate)
            .Get();

        var newRemoteEspers = response.Models;

        if (newRemoteEspers != null && newRemoteEspers.Any())
        {
            foreach (var remoteEsper in newRemoteEspers)
            {
                // Ensure it's marked clean locally since it just came from the server
                remoteEsper.IsLocalDirty = false;

                var existsLocally = await dbContext.OwnedEspers.AnyAsync(oe => oe.EsperId == remoteEsper.EsperId && oe.ProfileId == remoteEsper.ProfileId);

                if (existsLocally)
                {
                    dbContext.OwnedEspers.Update(remoteEsper);
                }
                else
                {
                    dbContext.OwnedEspers.Add(remoteEsper);
                }
            }

            await dbContext.SaveChangesAsync();
        }
    }

    /// <summary>
    /// Flushes the active SQLite database file to the browser's persistent IndexedDB.
    /// </summary>
    public async Task SaveDatabaseToBrowserAsync()
    {
        // Read the active SQLite file from the WASM virtual file system
        var dbBytes = System.IO.File.ReadAllBytes("nteplanner_local.db");

        // Pass it to your existing JS interop to save in IndexedDB
        await _jsRuntime.InvokeVoidAsync("indexedDbInterop.save", "nteplanner_local.db", dbBytes);
    }

    private async Task SyncBaseStaticDataAsync()
    {
        using var dbContext = await _dbFactory.CreateDbContextAsync();

        // 1. Check if we already have the base data. 
        if (await dbContext.Profiles.AnyAsync() && await dbContext.Espers.AnyAsync())
        {
            return;
        }

        Console.WriteLine("Fetching Base Data from Supabase...");

        // --- NEW: Fetch Hunter Levels ---
        var hunterLevelResponse = await _supabase.From<HunterLevel>().Get();
        if (hunterLevelResponse.Models.Any())
            dbContext.HunterLevels.AddRange(hunterLevelResponse.Models);

        // Fetch independent lookup tables
        var raritiesResponse = await _supabase.From<Rarity>().Get();
        if (raritiesResponse.Models.Any())
            dbContext.Rarities.AddRange(raritiesResponse.Models);

        var elementsResponse = await _supabase.From<Element>().Get();
        if (elementsResponse.Models.Any())
            dbContext.Elements.AddRange(elementsResponse.Models);

        var rolesResponse = await _supabase.From<CombatRole>().Get();
        if (rolesResponse.Models.Any())
            dbContext.CombatRoles.AddRange(rolesResponse.Models);

        var arcCompResponse = await _supabase.From<ArcCompatability>().Get();
        if (arcCompResponse.Models.Any())
            dbContext.ArcCompatabilities.AddRange(arcCompResponse.Models);

        // Save the lookups and Hunter Levels so they exist in the DB 
        // before we add things that depend on them!
        await dbContext.SaveChangesAsync();

        // --- NEW: Fetch Profiles ---
        // (In a production app, you might only fetch the active user's profile, 
        // but for testing, fetching all profiles is perfectly fine).
        var profileResponse = await _supabase.From<Profile>().Get();
        if (profileResponse.Models.Any())
            dbContext.Profiles.AddRange(profileResponse.Models);

        // Fetch dependent tables (Espers)
        var espersResponse = await _supabase.From<Esper>().Get();
        if (espersResponse.Models.Any())
        {
            dbContext.Espers.AddRange(espersResponse.Models);
        }

        // Final save for Profiles and Espers
        await dbContext.SaveChangesAsync();

        Console.WriteLine("Base Data seeded successfully!");
    }
}