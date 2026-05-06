using nte_planner.Models;

namespace nte_planner.Services;

public class SyncService
{

    public async Task SyncWithSupabaseAsync(LocalDbContext localDb, Supabase.Client supabase)
    {
        // --- 1. PUSH LOCAL CHANGES ---
        var unsyncedItems = localDb.Todos.Where(t => !t.IsSynced).ToList();

        if (unsyncedItems.Any())
        {
            // Push to Supabase using Upsert (Insert or Update)
            var response = await supabase.From<Todo>().Upsert(unsyncedItems);

            if (response.Models.Count > 0)
            {
                // Mark local items as synced
                foreach (var item in unsyncedItems)
                {
                    item.IsSynced = true;
                }
                await localDb.SaveChangesAsync();
            }
        }

        // --- 2. PULL REMOTE CHANGES ---
        // In a real app, you'd filter by UpdatedAt to only pull new records.
        var remoteData = await supabase.From<Todo>().Get();
        var remoteItems = remoteData.Models;

        foreach (var remoteItem in remoteItems)
        {
            var localItem = await localDb.Todos.FindAsync(remoteItem.Id);

            if (localItem == null)
            {
                // Doesn't exist locally, add it
                remoteItem.IsSynced = true;
                localDb.Todos.Add(remoteItem);
            }
            else if (remoteItem.UpdatedAt > localItem.UpdatedAt)
            {
                // Remote is newer, update local
                localItem.TaskName = remoteItem.TaskName;
                localItem.IsComplete = remoteItem.IsComplete;
                localItem.UpdatedAt = remoteItem.UpdatedAt;
                localItem.IsSynced = true;
            }
        }

        await localDb.SaveChangesAsync();
    }
}