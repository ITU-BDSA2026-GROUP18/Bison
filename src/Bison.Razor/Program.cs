using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

string? envPath = Environment.GetEnvironmentVariable("BISONDBPATH");
string dbPath = "../Database/";
if (string.IsNullOrWhiteSpace(envPath))
{
    dbPath = Path.Combine(Path.GetTempPath(), "bison.db");
}
else dbPath += envPath;
Console.WriteLine($"Using database: {Path.GetFullPath(dbPath)}");
// Load database connection via configuration
string connectionString = $"Data Source={dbPath}";

// Add services to the container.
builder.Services.AddDbContext<BisonDbContext>(options => options.UseSqlite(connectionString));
builder.Services.AddRazorPages();
builder.Services.AddScoped<DBFacade>();
builder.Services.AddScoped<IObservationService, ObservationService>();


var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<BisonDbContext>();
    dbContext.Database.Migrate();
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.MapRazorPages();

app.Run();
