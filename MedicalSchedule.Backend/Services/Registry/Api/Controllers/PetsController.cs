using Microsoft.AspNetCore.Mvc;

namespace Registry.Api.Controllers;

public class PetsController : Controller
{
    public IActionResult Index()
    {
        return View();
    }
}
