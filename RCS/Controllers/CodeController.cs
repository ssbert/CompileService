using Microsoft.AspNetCore.Mvc;
using RCS.Models;
using System.Diagnostics;

namespace RCS.Controllers
{
    public class CodeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
