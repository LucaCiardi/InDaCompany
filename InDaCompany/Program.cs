using InDaCompany.Data.Implementations;
using InDaCompany.Data.Interfaces;
using Microsoft.AspNetCore.Authentication.Cookies;

var builder = WebApplication.CreateBuilder(args);

string connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

builder.Services.AddControllersWithViews();

// Register DAOs
builder.Services.AddScoped<IDAOUtenti>(provider => new DAOUtenti(connectionString));
builder.Services.AddScoped<IDAOPost>(provider => new DAOPost(connectionString));
builder.Services.AddScoped<IDAOForum>(provider => new DAOForum(connectionString));
builder.Services.AddScoped<IDAOThreadForum>(provider => new DAOThreadForum(connectionString));
builder.Services.AddScoped<IDAOMessaggiThread>(provider => new DAOMessaggiThread(connectionString));
builder.Services.AddScoped<IDAOTicket>(provider => new DAOTicket(connectionString));
builder.Services.AddScoped<IDAOLikes>(provider => new DAOLikes(connectionString));

// Configure Authentication with secure cookie settings
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options => {
        options.LoginPath = "/Utenti/Login";
        options.LogoutPath = "/Utenti/Logout";
        options.AccessDeniedPath = "/Utenti/Login";
        options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
        options.Cookie.SameSite = SameSiteMode.Lax;
        options.Cookie.HttpOnly = true;
    });

var app = builder.Build();

// Configure CSP middleware
app.Use(async (context, next) =>
{
    context.Response.Headers.Add(
        "Content-Security-Policy",
        "default-src 'self'; " +
        "script-src 'self' 'unsafe-inline' 'unsafe-eval' https://cdn.jsdelivr.net/; " +
        "style-src 'self' 'unsafe-inline' https://cdn.jsdelivr.net/ https://fonts.googleapis.com; " +
        "img-src 'self' data: blob:; " +
        "font-src 'self' https://cdn.jsdelivr.net/ https://fonts.gstatic.com; " +
        "connect-src 'self'; " +
        "frame-ancestors 'none'; " +
        "form-action 'self'");

    await next();
});

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

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Utenti}/{action=Login}/{id?}");

app.Run();
