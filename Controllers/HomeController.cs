using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Http; 
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

using Cocodrinks.Models;
using Cocodrinks.Utilities;

namespace Cocodrinks.Controllers
{
    public class HomeController : Controller
    {
        private readonly CocodrinksContext _context;
        private readonly ILogger _logger;

        public HomeController(CocodrinksContext context, ILoggerFactory logger)
        {
            _context = context;
            _logger = logger.CreateLogger("Cocodrinks.Controllers.HomeController");
        }
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
            return View(new LoginViewModel());
        }

        [HttpPost]
        public IActionResult Login(LoginViewModel usercred)
        {
            if(Request.Method == "POST"){
                //Console.WriteLine(" got form post "+Request.Form["Name"].ToString());
                _logger.LogWarning(9, " got form post "+ Request.Form["Name"].ToString());
                if(usercred.Name != null && usercred.Name.Length > 0){
                    
                    var found = DbHelper.finduser(_context,usercred.Name);
                    if(found == true){
                        Console.WriteLine("{0} logged in", usercred.Name);
                    }else{
                        User user = new User();
                        user.Name = usercred.Name;
                        user.Password = usercred.Password;

                        _context.Users.Add(user);
                        _context.SaveChanges();
                        Console.WriteLine("{0} created ", user.Name);
                    }
                    HttpContext.Session.SetString("username", usercred.Name);
                    return this.Redirect("Index");
                }
            }
            return View(usercred);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
