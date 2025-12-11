using Microsoft.EntityFrameworkCore;
using SchoolSystem.Academic.Data;
using Microsoft.AspNetCore.Mvc;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "Academic Service API", Version = "v1" });
});

// DB Context
builder.Services.AddDbContext<AcademicDbContext>(options =>
    options.UseSqlite("Data Source=academic.db"));

var app = builder.Build();

// Auto-migrate - RESET DATABASE ON STARTUP (for testing)
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AcademicDbContext>();
    //db.Database.EnsureDeleted(); // Delete existing database
    //db.Database.EnsureCreated(); // Create fresh database
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapControllers();
app.Run();

