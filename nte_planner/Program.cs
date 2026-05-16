using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.JSInterop;
using nte_planner.Components;
using nte_planner.Data;
using Supabase;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

// 1. Register Local SQLite (EF Core)
// In Blazor WASM, SQLite runs in the browser via WebAssembly. 
// We create the file locally in the browser's virtual file system.
builder.Services.AddDbContextFactory<AppDbContext>(options =>
    options.UseSqlite("Data Source=nteplanner_local.db"));

builder.Services.AddScoped<nte_planner.Services.EsperDataService>();
builder.Services.AddScoped<nte_planner.Services.DataSyncService>();

// 2. Register Remote Supabase Client
// Replace with your actual Supabase URL and Anon Key (usually stored in appsettings.json or environment variables)
var supabaseUrl = builder.Configuration["Supabase:Url"] ?? "https://your-project.supabase.co";
var supabaseKey = builder.Configuration["Supabase:Key"] ?? "your-anon-key";

builder.Services.AddScoped(sp => new Supabase.Client(
    supabaseUrl,
    supabaseKey,
    new SupabaseOptions
    {
        AutoRefreshToken = true,
        AutoConnectRealtime = true
    }));

var host = builder.Build();

// 1. Pull the saved SQLite file from IndexedDB BEFORE EF Core tries to use it
var jsRuntime = host.Services.GetRequiredService<IJSRuntime>();
try
{
    var dbBytes = await jsRuntime.InvokeAsync<byte[]>("indexedDbInterop.load", "nteplanner_local.db");
    if (dbBytes != null)
    {
        // Place the database file back into Blazor's active memory
        File.WriteAllBytes("nteplanner_local.db", dbBytes);
    }
}
catch
{
    // Ignore the error if it's the very first time running and the DB doesn't exist yet
}

// 2. Now it is safe for EF Core to connect to the database
using (var scope = host.Services.CreateScope())
{
    var dbFactory = scope.ServiceProvider.GetRequiredService<IDbContextFactory<AppDbContext>>();
    using var dbContext = dbFactory.CreateDbContext();

    // Creates the schema if the file was totally empty
    await dbContext.Database.EnsureCreatedAsync();
}

await host.RunAsync();