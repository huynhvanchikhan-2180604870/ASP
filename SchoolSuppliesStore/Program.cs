using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileSystemGlobbing.Internal;
using SchoolSuppliesStore.Data;
using SchoolSuppliesStore.HelperClass;
using SchoolSuppliesStore.Models;
using SchoolSuppliesStore.Repositories;
using SchoolSuppliesStore.Service;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<ApplicationDbContext>(options =>options.UseSqlServer(builder.Configuration.GetConnectionString("SchoolSuppliesStore")));

builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
.AddDefaultTokenProviders()
.AddDefaultUI()
.AddEntityFrameworkStores<ApplicationDbContext>();
builder.Services.AddRazorPages();

// Add services to the container.
builder.Services.AddControllersWithViews().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.Preserve;
    options.JsonSerializerOptions.MaxDepth = 128;
});
builder.Services.AddScoped<IProductRepository, EFProductRepository>();
builder.Services.AddScoped<ICategoryRepository, EFCategoryRepository>();
builder.Services.AddScoped<IProductImagesRepository, EFProductImagesRepository>();
builder.Services.AddRazorComponents();
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

builder.Services.AddSingleton(x => new PaypalClient(
    builder.Configuration["Paypal:AppId"],
    builder.Configuration["Paypal:AppSecrect"],
    builder.Configuration["Paypal:Mode"]
));

builder.Services.AddSingleton<IVnPayService, VnPayService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
app.UseSession();
app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllerRoute(name:"admin", pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");
    endpoints.MapControllerRoute(name:"default", pattern: "{controller=Home}/{action=Index}/{id?}");
});
app.MapRazorPages();
app.Run();
