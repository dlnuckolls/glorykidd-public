using BlazorAppWeb.Components;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using BlazorAppWebMovies.Data;
using BlazorAppWeb.Data;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContextFactory<BlazorAppWebQuotesContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("BlazorAppWebQuotesContext") ?? throw new InvalidOperationException("Connection string 'BlazorAppWebQuotesContext' not found.")));
builder.Services.AddDbContextFactory<BlazorAppWebMoviesContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("BlazorAppWebMoviesContext") ?? throw new InvalidOperationException("Connection string 'BlazorAppWebMoviesContext' not found.")));

builder.Services.AddQuickGridEntityFrameworkAdapter();

builder.Services.AddDatabaseDeveloperPageExceptionFilter();

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    SeedData.Initialize(services);
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
    app.UseMigrationsEndPoint();
}

app.UseHttpsRedirection();


app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
