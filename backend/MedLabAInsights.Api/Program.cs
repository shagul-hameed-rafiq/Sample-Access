using MedLabAInsights.Data.Contexts;
using MedLabAInsights.Data.Seeding;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend",
        policy =>
        {
            policy.WithOrigins("http://localhost:4200", "https://localhost:4200")
                  .AllowAnyHeader()
                  .AllowAnyMethod();
        });
});

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<MedlabAinsightDbContext>(options =>
{
    var cs = builder.Configuration.GetConnectionString("Default");
    options.UseSqlite(cs, x => x.MigrationsAssembly("MedLabAInsights.Data"));
});


var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
if (app.Environment.IsDevelopment())
{
    using var scope = app.Services.CreateScope();
    var db = scope.ServiceProvider.GetRequiredService<MedlabAinsightDbContext>();

    await db.Database.MigrateAsync();
    await Seeder.SeedAsync(db);
}

app.UseCors("AllowFrontend");

if (!app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}
app.MapControllers();
app.Run();
