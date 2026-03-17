using ConstructionRental.Data;
using ConstructionRental.Services;
using ConstructionRental.BackgroundJobs;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.Cookies;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// ==========================================================
// 1. ĐĂNG KÝ DBCONTEXT (Kết nối SQL Server)
// ==========================================================
builder.Services.AddDbContext<ConstructionDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// ==========================================================
// 2. ĐĂNG KÝ CÁC SERVICES (Dependency Injection)
// ==========================================================
builder.Services.AddScoped<InventoryService>();
builder.Services.AddScoped<BillingService>();
builder.Services.AddScoped<ReturnService>();

// ==========================================================
// 3. ĐĂNG KÝ BACKGROUND JOB (Chạy ngầm tính phí trễ hạn)
// ==========================================================
builder.Services.AddHostedService<LateFeeCheckerJob>();

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Auth/Login"; // Đường dẫn đến trang đăng nhập
        options.AccessDeniedPath = "/Auth/AccessDenied"; // Đường dẫn khi bị từ chối truy cập
    });

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.Run();