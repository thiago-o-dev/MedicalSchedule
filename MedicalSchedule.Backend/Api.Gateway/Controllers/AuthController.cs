using Microsoft.AspNetCore.Mvc;

namespace Api.Gateway.Controllers;

public class AuthController : Controller
{
    public IActionResult Index()
    {
        return View();
    }
}
