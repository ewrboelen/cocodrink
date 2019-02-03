using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Cocodrinks.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.IO;
using Microsoft.AspNetCore.Hosting;

namespace Cocodrinks.Controllers.Api
{
    [Route("api/[controller]")]
    [ApiController]
    public class ArticleController : ControllerBase
    {
        private readonly CocodrinksContext _context;
        private readonly IHostingEnvironment _hostingEnvironment;
        private readonly ILogger<ArticleController> _logger;

        public ArticleController(CocodrinksContext context,IHostingEnvironment hostingEnvironment,ILogger<ArticleController> logger)
        {
            _context = context;
            _hostingEnvironment = hostingEnvironment;
            _logger = logger;
        }

         // GET: api/Article
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Article>>> GetArticles()
        {
            return await _context.Articles.ToListAsync();
        }

        // GET: api/Article/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Article>> GetArticle(long id)
        {
            var Article = await _context.Articles.FindAsync(id);
            return Article;
        }

        // POST: api/Article
        [HttpPost]
        public async Task<ActionResult<Article>> PostArticle(Article item)
        {
            _context.Articles.Add(item);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetArticle), new { id = item.Id }, item);
        }

        [HttpPost]
        [Route("Image")]
        public async Task PostImage(IFormFile file)
        {
            var filePath = _hostingEnvironment.WebRootPath+Path.DirectorySeparatorChar+"media"+Path.DirectorySeparatorChar;
            if (file.Length > 0)
            {
                using (var fileStream = new FileStream(Path.Combine(filePath, file.FileName), FileMode.Create))
                {
                    await file.CopyToAsync(fileStream);
                    _logger.LogInformation("---- saved file: "+file.Name+ " to "+filePath);
                }
            }
        }

        // PUT: api/Article/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutArticle(long id, Article item)
        {
            _context.Entry(item).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return NoContent();
        }
        
    }
}
