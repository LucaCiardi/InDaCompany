using Microsoft.AspNetCore.Mvc;
namespace InDaCompany.Controllers;

public abstract class BaseController : Controller
{
    protected readonly string ConnectionString;

    public BaseController(IConfiguration configuration)
    {
        ConnectionString = configuration.GetConnectionString("DefaultConnection")
                            ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
    }
}
