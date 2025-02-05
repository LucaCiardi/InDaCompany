using InDaCompany.Data.Implementations;
using InDaCompany.Data.Interfaces;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

builder.Services.AddScoped<IDAOUtenti, DAOUtenti>();
builder.Services.AddScoped<IDAOPost, DAOPost>();
builder.Services.AddScoped<IDAOForum, DAOForum>();
builder.Services.AddScoped<IDAOThreadForum, DAOThreadForum>();
builder.Services.AddScoped<IDAOMessaggiThread, DAOMessaggiThread>();
builder.Services.AddScoped<IDAOTicket, DAOTicket>();

builder.Services.AddAuthentication().AddCookie();

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

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
