using Microsoft.AspNetCore.Mvc;

namespace lab1.Controllers;

public class HomeController : Controller
{
    public IActionResult Index()
    {
        return View();
    }
}
