using Microsoft.EntityFrameworkCore;
using TicketAppWeb.Models.DataLayer;
using TicketAppWeb.Models.DataLayer.Reposetories;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Register database context
builder.Services.AddDbContext<TicketAppContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))
);

// Register repositories for dependency injection
builder.Services.AddScoped<IProjectRepository, ProjectRepository>(); // Register the Project Repository
builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>)); // Register the Generic Repository

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
