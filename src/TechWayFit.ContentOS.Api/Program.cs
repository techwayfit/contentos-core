var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();

var app = builder.Build();

// Configure the HTTP request pipeline
app.UseHttpsRedirection();

app.MapControllers();

app.MapGet("/", () => new 
{
    Name = "TechWayFit ContentOS Core API",
    Version = "1.0.0",
    Description = "Headless, API-first content management platform"
});

app.Run();
