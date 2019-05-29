using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using EcommerceProject.Models;
using EcommerceProject.Helper;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

namespace EcommerceProject.Controllers
{
    public class OrderItemsController : Controller
    {
        private readonly EcommerceProjectContext _context;

        public OrderItemsController(EcommerceProjectContext context)
        {
            _context = context;
        }

        // GET: OrderItems
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Index()
        {
            var ecommerceProjectContext = _context.OrderItem.Include(o => o.Order);
            return View(await ecommerceProjectContext.ToListAsync());
        }

        // GET: OrderItems/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var orderItem = await _context.OrderItem
                .Include(o => o.Order)
                .FirstOrDefaultAsync(m => m.ItemId == id);
            if (orderItem == null)
            {
                return NotFound();
            }

            return View(orderItem);
        }

        // GET: OrderItems/Create
        public IActionResult Create()
        {
            ViewData["OrderId"] = new SelectList(_context.Order, "OrderId", "OrderId");
            return View();
        }


       
        public async Task<IActionResult> CreateOrder(int id)
        {
            string sessionID = HttpContext.Session.Id;
            var userId = this.User.FindFirstValue(ClaimTypes.NameIdentifier);
            List<Product> cart = SessionHelper.GetObjectFromJson<List<Product>>(HttpContext.Session, sessionID);


            if (ModelState.IsValid)
            {
                for (int i = cart.Count() - 1; i >= 0; i--)
                {
                    OrderItem order = new OrderItem
                    {
                        OrderItemName = cart[i].ProductName,
                        OrderItemPrice = cart[i].ProductPrice,
                        OrderId = id,
                        
                    };
                    _context.Add(order);
                 
                  



                }
               

                await _context.SaveChangesAsync();
                SessionHelper.SetObjectAsJson(HttpContext.Session, sessionID, null);
                return RedirectToAction("Thanksfororder","Orders");
                
            }
            // ViewData["OrderId"] = new SelectList(_context.Order, "OrderId", "OrderId", orderItem2.OrderId);
            return RedirectToAction("Thanksfororder", "Orders");
        }

        // POST: OrderItems/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ItemId,OrderItemName,OrderItemPrice,OrderId")] OrderItem orderItem)
        {
       

            if (ModelState.IsValid)
            {
                _context.Add(orderItem);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["OrderId"] = new SelectList(_context.Order, "OrderId", "OrderId", orderItem.OrderId);
            return View(orderItem);
        }

        // GET: OrderItems/Edit/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var orderItem = await _context.OrderItem.FindAsync(id);
            if (orderItem == null)
            {
                return NotFound();
            }
            ViewData["OrderId"] = new SelectList(_context.Order, "OrderId", "OrderId", orderItem.OrderId);
            return View(orderItem);
        }

        // POST: OrderItems/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id, [Bind("ItemId,OrderItemName,OrderItemPrice,OrderId")] OrderItem orderItem)
        {
            if (id != orderItem.ItemId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(orderItem);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!OrderItemExists(orderItem.ItemId))
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
            ViewData["OrderId"] = new SelectList(_context.Order, "OrderId", "OrderId", orderItem.OrderId);
            return View(orderItem);
        }

        // GET: OrderItems/Delete/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var orderItem = await _context.OrderItem
                .Include(o => o.Order)
                .FirstOrDefaultAsync(m => m.ItemId == id);
            if (orderItem == null)
            {
                return NotFound();
            }

            return View(orderItem);
        }

        // POST: OrderItems/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var orderItem = await _context.OrderItem.FindAsync(id);
            _context.OrderItem.Remove(orderItem);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool OrderItemExists(int id)
        {
            return _context.OrderItem.Any(e => e.ItemId == id);
        }
    }
}
