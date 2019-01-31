using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Cocodrinks.Models;
using Cocodrinks.Utilities;
using Microsoft.Extensions.Logging;
//using System.Web;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace Cocodrinks.Controllers
{
    public class OrdersController : Controller
    {
        private readonly CocodrinksContext _context;
        private readonly ILogger<OrdersController> _logger;

        public OrdersController(CocodrinksContext context, ILogger<OrdersController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // GET: Orders
        public async Task<IActionResult> Index()
        {
            var cocodrinksContext = _context.Orders.Include(o => o.User);
            var userId = HttpContext.Session.GetString("userId");
            if(userId == null){
                return Redirect("/Home/Login");
            }
            ViewData["UserId"] = userId;
            int accessLevel = AccessHelper.getAccessLevel(_context,userId);
            if(accessLevel < 5){
                return View(await _context.Orders.ToListAsync());
            }else{
                return View(await _context.Orders
                    .Where(m => m.UserId == Int32.Parse(userId) )
                    .ToListAsync() );
            }
        }

        // GET: Orders/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var order = await _context.Orders
                .Include(o => o.User)
                //.Include("OrderLines")
                .FirstOrDefaultAsync(m => m.Id == id);
            if (order == null)
            {
                return NotFound();
            }

           
           
            // nasty trick to downcast.
            var serializedParent = JsonConvert.SerializeObject(order); 
            OrderView mOrder  = JsonConvert.DeserializeObject<OrderView>(serializedParent);
            mOrder.Articles = _context.Articles.ToList();
            mOrder.OrderLines = _context.OrderLines
                .Where(o => o.OrderId == mOrder.Id)
                .ToList();
            _logger.LogError("got orderlines... "+ order.OrderLines.Count());
            return View(mOrder);
        }

        // GET: Orders/Create
        public IActionResult Create()
        {
            Order order = new OrderView();
            ViewData["ArticleNames"] = new SelectList(_context.Articles, "Id", "Name");
            HttpContext.Session.Remove("orderlines");
            return View(order);
        }

        // POST: Orders/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        //public IActionResult Create([Bind("Id,Comment,DeliveryDate,addArticle,newLineArticleId,newLineQuantity")] OrderView order)
        public IActionResult Create(OrderView order)
        {
            if (ModelState.IsValid)
            {
                if(order.addArticle == null){
                    return Redirect("Checkout?Comment="+order.Comment+"&DeliveryDate="+order.DeliveryDate); 
                }else{
                    _logger.LogInformation("--- adding orderline");
                    addOrderLineToSession(order);
                }
            }
            order.Articles = _context.Articles.ToList();
            ViewData["ArticleNames"] = new SelectList(_context.Articles, "Id", "Name");
            order.OrderLines = getOrderLinesFromSession();
            //var article = _context.Articles.Find(order.newLineArticleId); 
            //line.Article = article;
            return View(order);
        }

        

        [HttpGet]
        public IActionResult Checkout()//[Bind("Comment")] Order order)
        {
            Order order = new Order();
            //order.Comment = Request.QueryString["Comment"];
            string val = HttpContext.Request.ToString(); //Request.QueryString["product_id"];
            _logger.LogInformation("-- step 2 "+Request.Query["Comment"]);
            order.Comment = Request.Query["Comment"];
            order.DeliveryDate = DateTime.Parse(Request.Query["DeliveryDate"]);
            return View(order);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Checkout([Bind("Id,Comment,BankAccount")] Order order)
        {
           // if (ModelState.IsValid){
                order.UserId =  Int32.Parse(HttpContext.Session.GetString("userId"));
                order.OrderLines = getOrderLinesFromSession();

                foreach(var logline in _context.ChangeTracker.Entries() ){ //.Where(e => e.State == EntityState.Added)
                    _logger.LogWarning(logline.Entity.ToString() );
                }
                _context.SaveChanges();

                _context.Add(order);
                foreach(var logline in _context.ChangeTracker.Entries() ){ //.Where(e => e.State == EntityState.Added)
                    _logger.LogError(logline.Entity.ToString() );
                }
                await _context.SaveChangesAsync();
                _logger.LogInformation("-- order created: "+order.Id);
                
                return RedirectToAction(nameof(Index));
           // }
          //  return View(order);
        }

        private void addOrderLineToSession(OrderView order)
        {
            var orderLines = getOrderLinesFromSession();
            OrderLine line = new OrderLine();
            line.ArticleId = order.newLineArticleId;
            line.Quantity   = order.newLineQuantity;
            orderLines.Add(line);
            HttpContext.Session.SetString("orderlines", JsonConvert.SerializeObject(orderLines));
        }

        private ICollection<OrderLine> getOrderLinesFromSession()
        {
            var orderLines = HttpContext.Session.GetString("orderlines");
            if(orderLines == null){
                return new HashSet<OrderLine>(); 
            }
            return JsonConvert.DeserializeObject<ICollection<OrderLine>>(orderLines);;
        }

        // GET: Orders/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var order = await _context.Orders.FindAsync(id);
            if (order == null)
            {
                return NotFound();
            }
            return View(order);
        }

        // POST: Orders/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Comment")] Order order)
        {
            if (id != order.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(order);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!OrderExists(order.Id))
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
            return View(order);
        }

        // GET: Orders/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var order = await _context.Orders
                .FirstOrDefaultAsync(m => m.Id == id);
            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }

        // POST: Orders/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var order = await _context.Orders.FindAsync(id);
            _context.Orders.Remove(order);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool OrderExists(int id)
        {
            return _context.Orders.Any(e => e.Id == id);
        }
    }
}
