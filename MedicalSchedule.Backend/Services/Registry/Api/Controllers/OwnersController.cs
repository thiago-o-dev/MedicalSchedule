using Microsoft.AspNetCore.Mvc;

namespace Registry.Api.Controllers;

public class OwnersController : Controller
{
    public IActionResult Index()
    {
        return View();
    }
}
