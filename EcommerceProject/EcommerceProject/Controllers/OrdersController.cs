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
using EcommerceProject.Services;
using MimeKit;
using Microsoft.AspNetCore.Authorization;
//using EcommerceProject.Services;

namespace EcommerceProject.Controllers
{
    public class OrdersController : Controller
    {
        private readonly EcommerceProjectContext _context;

        public OrdersController(EcommerceProjectContext context)
        {
            _context = context;
        }

        // GET: Orders
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Index()
        {
            return View(await _context.Order.ToListAsync());
        }
        public async Task<IActionResult> Thanksfororder()
        {
            return View();
        }

        // GET: Orders/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var order = await _context.Order
                .FirstOrDefaultAsync(m => m.OrderId == id);
            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }

        // GET: Orders/Create
        public IActionResult Create()
        {
            
            return View();
        }


        public async Task<IActionResult> MyPage()
        {
            var userId = this.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var order = await _context.Order
                .FirstOrDefaultAsync(m => m.Username == userId);

            var webCourseLab1Context = _context.Order.Where(x => x.Username == order.Username);
            if (order != null)
            {



                return View(await webCourseLab1Context.ToListAsync());
            }
            else
            {
                return View();

            }
            
        }

        public async Task<IActionResult> Vieworderitems(int? id,string username)
        {
            var userId = this.User.FindFirstValue(ClaimTypes.NameIdentifier);
            

            var order = await _context.OrderItem
                .FirstOrDefaultAsync(m => m.OrderId == id);

            var webCourseLab1Context = _context.OrderItem.Where(x => x.OrderId == order.OrderId);
            return View(await webCourseLab1Context.ToListAsync());
        }


        public IActionResult CreateOrder()
        {
            var userId = this.User.FindFirstValue(ClaimTypes.NameIdentifier);
           



            return View();
        }




        // POST: Orders/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.

        public async Task<IActionResult> PaypalCheckout(string name,string email,string surname)
        {

            
            Random r = new Random();
            int num = r.Next();

            var userId = this.User.FindFirstValue(ClaimTypes.NameIdentifier);

            string sessionID = HttpContext.Session.Id;

            List<Product> cart = SessionHelper.GetObjectFromJson<List<Product>>(HttpContext.Session, sessionID);
            Order order = new Order();



            int totalPrice = 0;

            for (int i = cart.Count() - 1; i >= 0; i--)
            {
                totalPrice = totalPrice + cart[i].ProductPrice;



            }
            if (userId == null)
            {
                 order = new Order
                {
                    OrderId = num,
                    TotalPrice = totalPrice,
                    FirstName = name,
                    SurName = surname,
                    Email = email,
                    Username = null,
                };
            }

            else
            {

                 order = new Order
                {
                    OrderId = num,
                    TotalPrice = totalPrice,
                    FirstName = name,
                    SurName = surname,
                    Email = email,
                    Username = userId,
                };
            }

            EmailService m = new EmailService();
            m.sendEmailAsync(order.Email);

            if (ModelState.IsValid)
            {

                _context.Add(order);
                await _context.SaveChangesAsync();
                return RedirectToAction("CreateOrder", "OrderItems", new { id = num });
             
            }

            return RedirectToAction("CreateOrder", "OrderItems", new { id = num });






        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateNewOrder(Order model)
        {
            Random r = new Random();
            int num = r.Next();
            var userId = this.User.FindFirstValue(ClaimTypes.NameIdentifier);

            string sessionID = HttpContext.Session.Id;

            Order order = new Order();
            List<Product> cart = SessionHelper.GetObjectFromJson<List<Product>>(HttpContext.Session, sessionID);

           



            int totalPrice = 0;

            for (int i = cart.Count() - 1; i >= 0; i--)
            {
                totalPrice = totalPrice + cart[i].ProductPrice;


              
            }

            if (userId == null)
            {
                order = new Order
                {
                    OrderId = num,
                    TotalPrice = totalPrice,
                    FirstName = model.FirstName,
                    SurName = model.SurName,
                    Email = model.Email,
                    Username = null,
                };
            }



            else
            {
                 order = new Order
                {
                    OrderId = num,
                    TotalPrice = totalPrice,
                    FirstName = model.FirstName,
                    SurName = model.SurName,
                    Email = model.Email,
                    Username = userId,
                };

            }
            EmailService m = new EmailService();
             m.sendEmailAsync(model.Email);
        





            if (ModelState.IsValid)
            {

                _context.Add(order);
                await _context.SaveChangesAsync();
                return RedirectToAction("CreateOrder", "OrderItems", new {id = num});
                
            }

            return RedirectToAction("CreateOrder", "OrderItems", new { id = num });
          
        }






        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("OrderId,TotalPrice,FirstName,SurName,Email")] Order order)
        {
         
            if (ModelState.IsValid)
            {
              
                _context.Add(order);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(order);
        }

        // GET: Orders/Edit/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var order = await _context.Order.FindAsync(id);
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
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id, [Bind("OrderId,TotalPrice,FirstName,SurName,Email")] Order order)
        {
            if (id != order.OrderId)
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
                    if (!OrderExists(order.OrderId))
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
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var order = await _context.Order
                .FirstOrDefaultAsync(m => m.OrderId == id);
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
            var order = await _context.Order.FindAsync(id);
            _context.Order.Remove(order);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool OrderExists(int id)
        {
            return _context.Order.Any(e => e.OrderId == id);
        }
    }
}
