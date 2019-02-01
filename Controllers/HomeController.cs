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
            _logger.LogInformation(" Hi "+HttpContext.Session.GetString("username")+" id "+HttpContext.Session.GetString("userId"));
            if( HttpContext.Session.GetString("userId") != null &&  HttpContext.Session.GetString("userId").Length > 0){
                ViewData["username"] = HttpContext.Session.GetString("username");
                ViewData["UserId"] = HttpContext.Session.GetString("userId");
                return View();
            }else{
                return this.Redirect("/Home/Login");
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
                _logger.LogInformation(9, " got form post "+ Request.Form["Name"].ToString());
                if(usercred.Name != null && usercred.Name.Length > 0){
                    User user =null;
                    var found = DbHelper.findUserId(_context,usercred.Name);
                    if(found > 0){
                        //check password
                        user = _context.Users
                            .Where(u => u.Id == found)
                            .First();
                        if(user !=null && user.Password == usercred.Password){    
                            _logger.LogInformation(usercred.Name+" logged in");
                        }else{
                            _logger.LogWarning("invalid password "+usercred.Password);
                            usercred.ErrorMessage = "Invalid password";
                            return View(usercred);
                        }
                    }else{
                        //TODO set in user controller...
                        user = new User();
                        user.Name = usercred.Name;
                        user.Password = usercred.Password;
                        user.AccessLevel = 10;
                        _logger.LogWarning("creating user "+usercred.Name);
                        _context.Users.Add(user);
                        _context.SaveChanges();
                        
                    }
                    HttpContext.Session.SetString("username", usercred.Name);
                    HttpContext.Session.SetString("userId",user.Id.ToString());
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
