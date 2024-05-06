using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Bislerium.Data;
using Bislerium.Areas.Identity.Data;
var builder = WebApplication.CreateBuilder(args);
var connectionString = builder.Configuration.GetConnectionString("BisleriumContextConnection") ?? throw new InvalidOperationException("Connection string 'BisleriumContextConnection' not found.");

builder.Services.AddDbContext<BisleriumContext>(options => options.UseSqlServer(connectionString));

builder.Services.AddDefaultIdentity<BisleriumUser>(options => options.SignIn.RequireConfirmedAccount = true).AddEntityFrameworkStores<BisleriumContext>();

// Add services to the container.
builder.Services.AddControllersWithViews();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers(); // Map controller routes
});

app.MapRazorPages();

app.Run();
