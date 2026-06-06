using Microsoft.AspNetCore.Mvc;

namespace Registry.Api.Controllers;

public class VetsController : Controller
{
    public IActionResult Index()
    {
        return View();
    }
}
