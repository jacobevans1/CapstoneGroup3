using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using TicketAppWeb.Models.Configuration;
using TicketAppWeb.Models.DataLayer;
using TicketAppWeb.Models.DataLayer.Repositories;
using TicketAppWeb.Models.DataLayer.Repositories.Interfaces;
using TicketAppWeb.Models.DomainModels;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddUserSecrets<Program>();

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

// Configure the database context
builder.Services.AddDbContext<TicketAppContext>(options =>
	options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))
);

builder.Services.AddIdentity<TicketAppUser, IdentityRole>(options => options.SignIn.RequireConfirmedAccount = true)
	.AddEntityFrameworkStores<TicketAppContext>()
	.AddDefaultTokenProviders();

builder.Services.Configure<IdentityOptions>(options =>
{
	// Password settings.
	options.Password.RequireDigit = true;
	options.Password.RequireLowercase = true;
	options.Password.RequireNonAlphanumeric = true;
	options.Password.RequireUppercase = true;
	options.Password.RequiredLength = 6;
	options.Password.RequiredUniqueChars = 1;

	// Lockout settings.
	options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
	options.Lockout.MaxFailedAccessAttempts = 5;
	options.Lockout.AllowedForNewUsers = true;

	// User settings.
	options.User.AllowedUserNameCharacters =
		"abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
	options.User.RequireUniqueEmail = true;
	options.SignIn.RequireConfirmedEmail = false;
});

builder.Services.ConfigureApplicationCookie(options =>
{
	// Cookie settings
	options.Cookie.HttpOnly = true;
	options.ExpireTimeSpan = TimeSpan.FromMinutes(5);

	options.LoginPath = "/Identity/Account/Login";
	options.AccessDeniedPath = "/Identity/Account/AccessDenied";
	options.SlidingExpiration = true;
});

builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IGroupRepository, GroupRepository>();
builder.Services.AddScoped<IProjectRepository, ProjectRepository>();
builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));

var app = builder.Build();

// Seed the Admin user
var scope = app.Services.CreateScope();
var services = scope.ServiceProvider;
var userManager = services.GetRequiredService<UserManager<TicketAppUser>>();
var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();

SeedData.Initialize(services, userManager, roleManager).Wait();
//SeedData.AddUsers(services, userManager, roleManager).Wait();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
	app.UseExceptionHandler("/Home/Error");
	app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages();

app.MapControllerRoute(
	name: "page_sort",
	pattern: "{controller}/{action}/page/{pagenumber}/size/{pagesize}/sort/{sortfield}/{sortdirection}");
app.MapControllerRoute(
	name: "default",
	pattern: "{controller=Login}/{action=Index}/{id?}/{slug?}");

app.Run();
