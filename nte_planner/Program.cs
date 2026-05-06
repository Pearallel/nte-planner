using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.JSInterop;
using nte_planner;
using nte_planner.Models;
using Supabase;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

SQLitePCL.Batteries_V2.Init();

builder.Services.AddDbContextFactory<LocalDbContext>(options =>
    options.UseSqlite("Data Source=localdb.db"));

// 1. Retrieve keys from appsettings.json
var supabaseUrl = builder.Configuration["Supabase:Url"];
var supabaseKey = builder.Configuration["Supabase:Key"];

// 2. Register Supabase Client
builder.Services.AddScoped(provider =>
{
    var options = new SupabaseOptions
    {
        AutoConnectRealtime = false
    };

    return new Supabase.Client(supabaseUrl, supabaseKey, options);
});

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

var app = builder.Build();

var js = app.Services.GetRequiredService<IJSRuntime>();

// 1. Ask JS to pull the database from persistent storage
var dbBytes = await js.InvokeAsync<byte[]>("sqliteStorage.load", "localdb.db");
if (dbBytes != null)
{
    // 2. If it exists, write it into Blazor's virtual file system
    File.WriteAllBytes("localdb.db", dbBytes);
}

var dbFactory = app.Services.GetRequiredService<IDbContextFactory<LocalDbContext>>();
using var db = await dbFactory.CreateDbContextAsync();

await db.Database.EnsureCreatedAsync();

// 3. Initialize the Supabase Client
var supabaseClient = app.Services.GetRequiredService<Supabase.Client>();
await supabaseClient.InitializeAsync();

await app.RunAsync();