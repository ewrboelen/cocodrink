using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Http; 
using Microsoft.EntityFrameworkCore;

using Cocodrinks.Models;
using Cocodrinks.Utilities;

namespace Cocodrinks.Controllers
{
    public class HomeController : Controller
    {
        private readonly CocodrinksContext _context;

        public HomeController(CocodrinksContext context)
        {
            _context = context;
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

        public IActionResult Login(LoginViewModel usercred)
        {
            if(Request.Method == "POST"){
                Console.WriteLine(" got form post "+Request.Form["Name"].ToString());
                if(usercred.Name != null && usercred.Name.Length > 0){
                    
                    var found = DbHelper.finduser(_context,usercred.Name);
                    if(found == true){
                        Console.WriteLine("{0} logged in", usercred.Name);
                    }else{
                        User user = new User();
                        user.Name = usercred.Name;
                        user.Password = usercred.Password;

                        _context.User.Add(user);
                        _context.SaveChanges();
                        Console.WriteLine("{0} created ", user.Name);
                    }
                    HttpContext.Session.SetString("username", usercred.Name);
                    return this.Redirect("Index");
                }
            }
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
