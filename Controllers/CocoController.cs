using Microsoft.AspNetCore.Mvc;
using System.Text.Encodings.Web;

namespace Cocodrinks.Controllers
{
    public class CocoController : Controller
    {
        // 
        // GET: /Coco/

        public IActionResult Index()
        {
            return View();
        }

        // 
        // GET: /Coco/Welcome/ 

        public string Welcome()
        {
            return "This is the Welcome action method...";
        }
    }
}