using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using EcommerceProject.Models;
using System.Security.Claims;
using EcommerceProject.Helper;
using System.Reflection;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using System.IO;
using Microsoft.AspNetCore.Hosting;

namespace EcommerceProject.Controllers
{
    public class ProductsController : Controller
    {
        private readonly EcommerceProjectContext _context;
        private IHostingEnvironment he;

        public ProductsController(EcommerceProjectContext context, IHostingEnvironment he)
        {
            _context = context;
            this.he = he;
        }

        // GET: Products
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Index()
        {
            return View(await _context.Product.ToListAsync());
        }

        // GET: Products, MainCategory

        public async Task<IActionResult> SearchProduct(string id)
        {

            try
            {
                var product = await _context.Product
                    .FirstOrDefaultAsync(m => m.ProductName == id);

                var webCourseLab1Context = _context.Product.Where(x => x.ProductName == product.ProductName);
                return View(await webCourseLab1Context.ToListAsync());

            } catch(Exception e)
            {
                return View();
            }
        }

        public async Task<IActionResult> ProductViewCategory(string id)
        {
            

            var product = await _context.Product
                .FirstOrDefaultAsync(m => m.ProductCategory == id);

            var webCourseLab1Context = _context.Product.Where(x => x.ProductCategory == product.ProductCategory);
            return View(await webCourseLab1Context.ToListAsync());
        }

        public async Task<IActionResult> Subcategory(string id)
        {

           

            var product = await _context.Product
                .FirstOrDefaultAsync(m => m.ProductSubCategory == id);

            if(product == null)
            {
                return RedirectToAction("Index", "Home");
            }


            var webCourseLab1Context = _context.Product.Where(x => x.ProductSubCategory == product.ProductSubCategory);
            return View(await webCourseLab1Context.ToListAsync());
        }


        public ActionResult Buy(int? id, string sub)
        {
           

            var product = _context.Product
             .FirstOrDefault(m => m.ProductId == id);

            string sessionID = HttpContext.Session.Id;

            if (SessionHelper.GetObjectFromJson<List<Product>>(HttpContext.Session,sessionID) == null)
                {
                   
                    List<Product> cart = new List<Product>();
                    cart.Add(product);
                    SessionHelper.SetObjectAsJson(HttpContext.Session, sessionID, cart);
                    
                }
                else
                {
                    List<Product> cart = SessionHelper.GetObjectFromJson<List<Product>>(HttpContext.Session,sessionID);
                    cart.Add(product);
                    

                    SessionHelper.SetObjectAsJson(HttpContext.Session,sessionID, cart);
                }
                if (sub != null)
                {
                    return RedirectToAction("Subcategory", new { id = product.ProductSubCategory });
                }
            

                return RedirectToAction("ProductViewCategory", new { id = product.ProductCategory });
            }

        

        public ActionResult Cartpage()
        {
            // var userId = this.User.FindFirstValue(ClaimTypes.NameIdentifier);

            string sessionID = HttpContext.Session.Id;

            List<Product> cart = SessionHelper.GetObjectFromJson<List<Product>>(HttpContext.Session, sessionID);
         

            if (cart != null)
            {

                




                return View(cart);
            }
            else
            {
                return View();
                // return RedirectToAction("Index", "Home"); 
            }
        }

        public ActionResult removeFromCart(int? id)
        {
            var userId = this.User.FindFirstValue(ClaimTypes.NameIdentifier);
            string sessionID = HttpContext.Session.Id;

          
            
            List<Product> cart = SessionHelper.GetObjectFromJson<List<Product>>(HttpContext.Session, sessionID);


            for (int i = cart.Count() - 1; i >= 0; i--)
            {
                var item = cart[i];
                if (item.ProductId == id)
                {
                    cart.Remove(item);
                    break;
                }
            }

            SessionHelper.SetObjectAsJson(HttpContext.Session, sessionID, cart);

           

            return RedirectToAction("CartPage", "Products");
        }

        // GET: Products/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Product
                .FirstOrDefaultAsync(m => m.ProductId == id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // GET: Products/Create
        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Products/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create([Bind("ProductId,ProductName,ProductPrice,ProductCategory,ProductSubCategory")] Product product, IList<IFormFile> fil)
        {
           

            if (ModelState.IsValid)

            {



                var files = HttpContext.Request.Form.Files;
                foreach (var Image in files)
                {
                    if (Image != null && Image.Length > 0)
                    {
                        var file = Image;
                        //There is an error here
                        var uploads = Path.Combine(he.WebRootPath, "wwwroot\\images\\", file.FileName);
                       
                        if (file.Length > 0)
                        {
                            var fileName = Guid.NewGuid().ToString().Replace("-", "") + Path.GetExtension(file.FileName);
                            using (var stream = new FileStream(uploads, FileMode.Create))
                            {
                                await file.CopyToAsync(stream);
                               
                            }

                        }
                    }
                }


                _context.Add(product);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(product);
        }

        // GET: Products/Edit/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Product.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            return View(product);
        }

        // POST: Products/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id, [Bind("ProductId,ProductName,ProductPrice,ProductCategory,ProductSubCategory")] Product product)
        {
            if (id != product.ProductId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(product);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProductExists(product.ProductId))
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
            return View(product);
        }

        // GET: Products/Delete/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Product
                .FirstOrDefaultAsync(m => m.ProductId == id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // POST: Products/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var product = await _context.Product.FindAsync(id);
            _context.Product.Remove(product);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ProductExists(int id)
        {
            return _context.Product.Any(e => e.ProductId == id);
        }
    }
}
