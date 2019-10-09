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

namespace Cocodrinks.Controllers.Api
{
    [Route("api/[controller]")]
    [ApiController]
    public class OnderdelenController : ControllerBase
    {
        private readonly CocodrinksContext _context;
        //private readonly ILogger<OnderdelenController> _logger;

        public OnderdelenController(CocodrinksContext context,ILogger<OnderdelenController> logger)
        {
            _context = context;
           // _logger = logger;
        }

         // GET: api/Onderdelen
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Onderdelen>>> GetOnderdelen()
        {
            return await _context.Onderdelen.ToListAsync();
        }

        // GET: api/Onderdelen/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Onderdelen>> GetOnderdeel(long id)
        {
            var todoItem = await _context.Onderdelen.FindAsync(id);

            if (todoItem == null)
            {
                return NotFound();
            }

            return todoItem;
        }

        // GET: api/Onderdelen/5/
        [HttpGet("{id}/take")]
        public async Task<ActionResult<Onderdelen>> GetOnderdeelUitVoorraad(long id)
        {
            var todoItem = await _context.Onderdelen.FindAsync(id);

            if (todoItem == null)
            {
                return NotFound();
            }
            todoItem.hoeveelheid--;
            await _context.SaveChangesAsync();
            return todoItem;
        }

        
    }
}
