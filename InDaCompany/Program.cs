using InDaCompany.Data.Implementations;
using InDaCompany.Data.Interfaces;
using Microsoft.AspNetCore.Authentication.Cookies;

var builder = WebApplication.CreateBuilder(args);

string connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

builder.Services.AddControllersWithViews();

builder.Services.AddScoped<IDAOUtenti>(provider => new DAOUtenti(connectionString));
builder.Services.AddScoped<IDAOForum>(provider => new DAOForum(connectionString));
builder.Services.AddScoped<IDAOThreadForum>(provider => new DAOThreadForum(connectionString));
builder.Services.AddScoped<IDAOMessaggiThread>(provider => new DAOMessaggiThread(connectionString));
builder.Services.AddScoped<IDAOTicket>(provider => new DAOTicket(connectionString));
builder.Services.AddScoped<IDAOLikes>(provider => new DAOLikes(connectionString));

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options => {
        options.LoginPath = "/Utenti/Login";
        options.LogoutPath = "/Utenti/Logout";
        options.AccessDeniedPath = "/Home/AccessDenied";  
        options.ExpireTimeSpan = TimeSpan.FromHours(8);   
        options.SlidingExpiration = true;                
    });
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AllUsers", policy =>
        policy.RequireRole("Admin", "Manager", "Dipendente"));
});

var app = builder.Build();

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
    pattern: "{controller=Home}/{action=Index}/{id?}"); 

app.Run();