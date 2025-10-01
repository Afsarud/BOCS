using BOCS.Data;
using BOCS.Models;
using BOCS.Services;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

// ---------- Added by Afsar ----------
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddIdentity<Users, IdentityRole>(options =>
{
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequiredLength = 6;
    options.User.RequireUniqueEmail = true;
    options.SignIn.RequireConfirmedAccount = false;
    options.SignIn.RequireConfirmedEmail = false;
    options.SignIn.RequireConfirmedPhoneNumber = false;
})
.AddEntityFrameworkStores<AppDbContext>()
.AddDefaultTokenProviders();

// Student "My Course" menu helper
builder.Services.AddScoped<IEnrollmentService, EnrollmentService>();

// Auth cookie (session-like)
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Account/Login";
    options.LogoutPath = "/Account/Logout";
    options.SlidingExpiration = false;
    options.Cookie.HttpOnly = true;
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
    options.Cookie.SameSite = SameSiteMode.Strict;
});

if (builder.Environment.IsDevelopment())
{
    builder.Services.AddDataProtection().UseEphemeralDataProtectionProvider();
}
// ---------- end by Afsar ----------

var app = builder.Build();

// Seed (roles/users)
using (var scope = app.Services.CreateScope())
{
    await SeedService.SeedDatabase(scope.ServiceProvider);
}

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

// ---------- Security headers + CSP (BEFORE static files) ----------
app.Use(async (ctx, next) =>
{
    // Basic security headers
    ctx.Response.Headers["X-Content-Type-Options"] = "nosniff";
    ctx.Response.Headers["X-Frame-Options"] = "DENY";
    ctx.Response.Headers["Referrer-Policy"] = "strict-origin-when-cross-origin";
    ctx.Response.Headers["X-XSS-Protection"] = "0"; // modern browsers ignore/obsolete
    ctx.Response.Headers["Cache-Control"] = "no-store, no-cache, must-revalidate";
    ctx.Response.Headers["Pragma"] = "no-cache";

    // CSP: YouTube + gstatic allowed; nocookie used by player
    // Note: style-src 'unsafe-inline' রাখা হয়েছে Bootstrap inline styles-এর জন্য।
    var cspBase =
        "default-src 'self'; " +
        "img-src 'self' data: https://i.ytimg.com https:; " +
        "style-src 'self' 'unsafe-inline'; " +
        "font-src 'self' data:; " +
        "script-src 'self' https://www.youtube.com https://www.gstatic.com https://www.youtube-nocookie.com; " +
        "frame-src 'self' https://www.youtube.com https://www.youtube-nocookie.com; " +
        "child-src https://www.youtube.com https://www.youtube-nocookie.com; " +
        "media-src 'self' blob:; " +
        "worker-src 'self' blob:; ";

    if (app.Environment.IsDevelopment())
    {
        // Dev: allow localhost/ws for Hot Reload/webpack etc.
        cspBase += "connect-src 'self' http://localhost:* https://localhost:* ws://localhost:* wss://localhost:*;";
    }
    else
    {
        cspBase += "connect-src 'self';";
    }

    ctx.Response.Headers["Content-Security-Policy"] = cspBase;

    await next();
});

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseMiddleware<BOCS.Middleware.SingleSessionMiddleware>();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Account}/{action=Login}/{id?}");

app.Run();
