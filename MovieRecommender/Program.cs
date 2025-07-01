using DotNetEnv;
using Microsoft.AspNetCore.Authentication;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using MovieRecommender.Contexts;
using MovieRecommender.Handler;
using MovieRecommender.Interfaces;
using MovieRecommender.Services;
using Scalar.AspNetCore;
using TMDbLib.Client;
using AuthenticationService = MovieRecommender.Services.AuthenticationService;

Env.Load();

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddControllers();

// Add openapi service
builder.Services.AddOpenApi(options =>
{
    options.AddDocumentTransformer((document, _, _) =>
    {
        document.Components ??= new OpenApiComponents();
        document.Components.SecuritySchemes ??= new Dictionary<string, OpenApiSecurityScheme>();

        document.Components.SecuritySchemes["Bearer"] = new OpenApiSecurityScheme
        {
            Type = SecuritySchemeType.Http,
            Scheme = "bearer",
            Description = "Enter your user token"
        };

        return Task.CompletedTask;
    });
});

// Add EF Core DbContext with SQLite
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

// Add this to your service registration section
builder.Services.AddHttpClient();
builder.Services.AddSingleton<IWordListService, WordListService>();

builder.Services.AddSingleton<TMDbClient>(_ =>
{
    var apiKey = Environment.GetEnvironmentVariable("TMDB_API_KEY");

    if (string.IsNullOrEmpty(apiKey))
    {
        throw new InvalidOperationException(
            "TMDB API Key is not configured. Please set \"TMDB_API_KEY\" env variable.");
    }

    return new TMDbClient(apiKey);
});

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

builder.Services.AddScoped<TmdbService>();

builder.Services.AddScoped<AuthenticationService>();

builder.Services.AddScoped<RecommendationService>();

builder.Services.AddScoped<UserService>();

builder.Services.AddScoped<WatchlistService>();

// Add minimal authentication/authorization for OpenAPI metadata only
builder.Services.AddAuthentication("Bearer")
    .AddScheme<AuthenticationSchemeOptions, NoOpAuthenticationHandler>("Bearer", options => { });

builder.Services.AddAuthorization();

var app = builder.Build();

// Initialize the text resource service
using (var scope = app.Services.CreateScope())
{
    var textResourceService = scope.ServiceProvider.GetRequiredService<IWordListService>();
    await textResourceService.InitializeAsync();
}

// Apply migrations on start-up
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    dbContext.Database.Migrate();
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

app.UseHttpsRedirection();
app.UseRouting();

app.MapStaticAssets();

app.MapControllerRoute(
        name: "default",
        pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.MapControllers();

// Map OpenApi
app.MapOpenApi();
app.MapScalarApiReference("/api");

// Add authentication/authorization middleware
app.UseAuthentication();
app.UseAuthorization();
app.UseSession();

app.Run();