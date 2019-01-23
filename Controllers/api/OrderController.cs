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
    public class OrderController : ControllerBase
    {
        private readonly CocodrinksContext _context;
        private readonly ILogger<OrderController> _logger;

        public OrderController(CocodrinksContext context,ILogger<OrderController> logger)
        {
            _context = context;
            _logger = logger;
        }

         // GET: api/Order
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Order>>> GetOrders()
        {
            return await _context.Orders.ToListAsync();
        }

        // GET: api/Order/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Order>> GetOrder(long id)
        {
            var order = await _context.Orders.FindAsync(id);

            if (order == null)
            {
                return NotFound();
            }

            return order;
        }

        
    }
}
