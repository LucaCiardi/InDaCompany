using InDaCompany.Data.Implementations;
using InDaCompany.Data.Interfaces;

var builder = WebApplication.CreateBuilder(args);

string connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

builder.Services.AddControllersWithViews();

builder.Services.AddScoped<IDAOUtenti>(provider => new DAOUtenti(connectionString));
builder.Services.AddScoped<IDAOPost>(provider => new DAOPost(connectionString));
builder.Services.AddScoped<IDAOForum>(provider => new DAOForum(connectionString));
builder.Services.AddScoped<IDAOThreadForum>(provider => new DAOThreadForum(connectionString));
builder.Services.AddScoped<IDAOMessaggiThread>(provider => new DAOMessaggiThread(connectionString));
builder.Services.AddScoped<IDAOTicket>(provider => new DAOTicket(connectionString));
builder.Services.AddScoped<IDAOLikes>(provider => new DAOLikes(connectionString));

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
    pattern: "{controller=Utenti}/{action=Login}/{id?}");

app.Run();
