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
    public class UserController : ControllerBase
    {
        private readonly CocodrinksContext _context;
        private readonly ILogger<UserController> _logger;

        public UserController(CocodrinksContext context,ILogger<UserController> logger)
        {
            _context = context;
            _logger = logger;
        }

         // GET: api/User
        [HttpGet]
        public async Task<ActionResult<IEnumerable<User>>> GetUsers()
        {
            return await _context.Users.ToListAsync();
        }

        // GET: api/User/5
        [HttpGet("{id}")]
        public async Task<ActionResult<User>> GetUser(long id)
        {
            var todoItem = await _context.Users.FindAsync(id);

            if (todoItem == null)
            {
                return NotFound();
            }

            return todoItem;
        }

        
    }
}
