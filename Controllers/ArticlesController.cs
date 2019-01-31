using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Cocodrinks.Models;
using Cocodrinks.Utilities;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;

namespace Cocodrinks.Controllers
{
    public class ArticlesController : Controller
    {
        private readonly CocodrinksContext _context;
        private readonly IHostingEnvironment _hostingEnvironment;
        private readonly ILogger<ArticlesController> _logger;

        public ArticlesController(CocodrinksContext context,IHostingEnvironment hostingEnvironment, ILogger<ArticlesController> logger)
        {
            _context = context;
            _hostingEnvironment = hostingEnvironment;
            _logger = logger;
        }

        // GET: Articles
        public async Task<IActionResult> Index()
        {
            var userId = HttpContext.Session.GetString("userId");
            int accessLevel = AccessHelper.getAccessLevel(_context,userId);
            if(accessLevel < 5){
                return View("AdminIndex",await _context.Articles.ToListAsync());
            }else{
                return View(await _context.Articles.ToListAsync());
            }

        }

        // GET: Articles/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var article = await _context.Articles
                .FirstOrDefaultAsync(m => m.Id == id);
            if (article == null)
            {
                return NotFound();
            }

            return View(article);
        }

        // GET: Articles/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Articles/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Description,Imagelocation")] Article article)
        {
            if (ModelState.IsValid)
            {
                _context.Add(article);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(article);
        }

        // GET: Articles/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var article = await _context.Articles.FindAsync(id);
            if (article == null)
            {
                return NotFound();
            }
            return View(article);
        }

        // POST: Articles/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Description,Imagelocation")] Article article)
        {
            if (id != article.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(article);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ArticleExists(article.Id))
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
            return View(article);
        }

        // GET: Articles/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var article = await _context.Articles
                .FirstOrDefaultAsync(m => m.Id == id);
            if (article == null)
            {
                return NotFound();
            }

            return View(article);
        }

        // POST: Articles/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var article = await _context.Articles.FindAsync(id);
            _context.Articles.Remove(article);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ArticleExists(int id)
        {
            return _context.Articles.Any(e => e.Id == id);
        }
    // GET: Articles/Upload
        public IActionResult Upload()
        {
            return View();
        }

    [HttpPost] //("Upload")
    public async Task<IActionResult> Upload(List<IFormFile> files)
    {
        long size = files.Sum(f => f.Length);

        // full path to file in temp location
        var filePath = _hostingEnvironment.WebRootPath+"\\media\\"; //Path.GetTempFileName();
        _logger.LogInformation("---- saving "+files.Count()+ " files to "+filePath);
        foreach (var formFile in files)
        {
            if (formFile.Length > 0)
            {
                _logger.LogInformation("-- saving file to "+filePath+formFile.FileName);
                using (var stream = new FileStream(filePath+formFile.FileName, FileMode.Create))
                {
                    await formFile.CopyToAsync(stream);
                }

            }
        }
        _logger.LogInformation("-- saving files done");

        
        //return Ok(new { count = files.Count, size, filePath});
        ViewData["Message"] = files.Count()+" Files have been uploaded";
        return View();
    }


    }


}
