using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using TicketAppWeb.Models;
using TicketAppWeb.Models.DataLayer;
using TicketAppWeb.Models.DomainModels;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Configure the database context
builder.Services.AddDbContext<TicketAppContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))
);

// Register Identity Services
builder.Services.AddIdentity<TicketAppUser, IdentityRole>()
    .AddEntityFrameworkStores<TicketAppContext>()
    .AddDefaultTokenProviders();

// Enable authentication & authorization
builder.Services.AddAuthentication();
builder.Services.AddAuthorization();

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

// Enable authentication & authorization middleware
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();