using MedLabAInsights.Data.Contexts;
using MedLabAInsights.Data.Seeding;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// ✅ CORS — allow Angular frontend (local dev + Render production)
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy
            .WithOrigins(
                "http://localhost:4200",
                "https://medlab-frontend.onrender.com"
            )
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

// ✅ PostgreSQL instead of SQLite
builder.Services.AddDbContext<MedlabAinsightDbContext>(options =>
{
    var cs = builder.Configuration.GetConnectionString("Default");
    options.UseNpgsql(cs, x => x.MigrationsAssembly("MedLabAInsights.Data"));
});

var app = builder.Build();

// Swagger (optional but fine)
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// ✅ Always run migrations (important for Render)
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<MedlabAinsightDbContext>();
    await db.Database.MigrateAsync();
    await Seeder.SeedAsync(db);
}

// ✅ CORS must be before MapControllers
app.UseCors("AllowFrontend");

app.UseHttpsRedirection();
app.MapGet("/", () => Results.Ok(new { status = "ok" }));
app.MapControllers();

// ✅ CRITICAL for Render
var port = Environment.GetEnvironmentVariable("PORT") ?? "8080";
app.Urls.Add($"http://0.0.0.0:{port}");

app.Run();
