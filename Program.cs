using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using sheep.Data;
using WebApplication5.Services;

var builder = WebApplication.CreateBuilder(args);

var connectionString =
    Environment.GetEnvironmentVariable("Pass")
    ?? builder.Configuration.GetConnectionString("DefaultConnection");
var user = Environment.GetEnvironmentVariable("SMTP_USER");
var password = Environment.GetEnvironmentVariable("SMTP_PASSWORD");

builder.WebHost.UseUrls($"http://0.0.0.0:{Environment.GetEnvironmentVariable("PORT") ?? "8080"}");

builder.Services.ConfigureApplicationCookie(options =>
{
    options.Cookie.HttpOnly = true;
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always; // HTTPSを強制
    options.Cookie.SameSite = SameSiteMode.Lax;
});

builder.Services.AddTransient<IEmailSender, EmailSender>();
builder.Services.AddHostedService<DailyEmailHostedService>();
builder.Services.AddScoped<EmailService>();
builder.WebHost.UseUrls("http://+:10000");

// 1️⃣ データベース接続
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// 2️⃣ Identity
builder.Services.AddDefaultIdentity<IdentityUser>(options =>
{
    options.SignIn.RequireConfirmedAccount = false;
    options.Password.RequireDigit = false;          // 数字必須 → なし
    options.Password.RequireLowercase = false;      // 小文字必須 → なし
    options.Password.RequireUppercase = false;      // 大文字必須 → なし
    options.Password.RequireNonAlphanumeric = false; // 記号必須 → なし
    options.Password.RequiredLength = 4;
    options.SignIn.RequireConfirmedAccount = false;　// メール確認不要
})
    .AddEntityFrameworkStores<ApplicationDbContext>();

// 3️⃣ Razor Pages
builder.Services.AddRazorPages();


var app = builder.Build();

// 4️⃣ ミドルウェア
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication(); // これ必須
app.UseAuthorization();

app.MapRazorPages();

app.Run();
