using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Cocodrinks.Models;
using Cocodrinks.Utilities;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Cocodrinks.Controllers
{
    public class UserController : Controller
    {
        private readonly CocodrinksContext _context;
        private readonly ILogger<UserController> _logger;

        public UserController(CocodrinksContext context,ILogger<UserController> logger)
        {
            _context = context;
            _logger = logger;
        }

        public IActionResult Logout()
        {
            _logger.LogInformation("logging out "+ HttpContext.Session.GetString("username"));
            HttpContext.Session.SetString("username","");
            HttpContext.Session.Clear();
            return this.Redirect("/Home/Login");
            //return View();
        }

        // GET: User
        public async Task<IActionResult> Index()
        {
             var userId = HttpContext.Session.GetString("userId");
            ViewData["UserId"] = userId;
            int accessLevel = AccessHelper.getAccessLevel(_context,userId);
            if(accessLevel < 5){
                _logger.LogInformation("view userlist by "+userId+" with lvl "+accessLevel);
                return View(await _context.Users.ToListAsync());
            }else{
                return View(await _context.Users
                    .Where(m => m.Id == Int32.Parse(userId) )
                    .ToListAsync() );
            }
        }

        // GET: User/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                var sid = HttpContext.Session.GetString("userId");
                if(sid == null){
                    return NotFound();
                }
                return this.Redirect("/User/Details/"+sid);
            }

            var user = await _context.Users
                .FirstOrDefaultAsync(m => m.Id == id);
            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        // GET: User/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: User/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Password,CreateDate,Logincount")] User user)
        {
            user.AccessLevel = 10;
            if (ModelState.IsValid)
            {
                user.AccessLevel = 10;
                _logger.LogInformation("creating user "+user.Name+" with lvl "+user.AccessLevel);
                _context.Add(user);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(user);
        }

        // GET: User/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            return View(user);
        }

        // POST: User/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Password,CreateDate,Logincount")] User user)
        {
            if (id != user.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(user);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UserExists(user.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(user);
        }

        // GET: User/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _context.Users
                .FirstOrDefaultAsync(m => m.Id == id);
            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        // POST: User/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var user = await _context.Users.FindAsync(id);
            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool UserExists(int id)
        {
            return _context.Users.Any(e => e.Id == id);
        }
    }
}
