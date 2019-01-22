using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Cocodrinks.Models;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Http; 

namespace Cocodrinks.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            Console.WriteLine(" Hi "+HttpContext.Session.GetString("username"));
            if( HttpContext.Session.GetString("username") != null &&  HttpContext.Session.GetString("username").Length > 0){
                ViewData["username"] = HttpContext.Session.GetString("username");
                return View();
            }else{
                return this.Redirect("Home/Login");
            }
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult Login()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
