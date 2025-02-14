using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using UsermmanagementWithIdentity.Data;
using UsermmanagementWithIdentity.Models;

namespace UsermmanagementWithIdentity.Controllers
{
    public class ProductController : Controller
    {
        private readonly ApplicationDbContext _context;
        public ProductController(ApplicationDbContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            var products = _context.Products.ToList();
            return View(products);
        }
        [HttpGet]
        public IActionResult Add()
        {
            var product = new Product();
            return View(product);
        }
        [HttpPost]
        public IActionResult Add(Product product)
        {
            if (!ModelState.IsValid)
            {
                return View(product);
            }
            _context.Products.Add(product);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }
        [HttpGet]
        public IActionResult Edit(int id)
        {
            var product = _context.Products.Find(id);
            return View(product);
        }
        [HttpPost]
        public IActionResult Edit(Product product)
        {
            if (!ModelState.IsValid)
            {
                return View(product);
            }
            _context.Products.Update(product);

            _context.SaveChanges();
            return RedirectToAction("Index");
        }
        [HttpPost]

        public IActionResult Delete(int id)
        {
            try
            {
                var product = _context.Products.Find(id);
                if (product == null)
                {
                    return Json(new { success = false, message = "The product is not found in database" });
                }
                _context.Products.Remove(product);
                _context.SaveChanges();
                return Json(new { success = true, message = "Done" });
            }
            catch (Exception ex)
            {
                // If something goes wrong
                return Json(new { success = false, message = ex.Message });
            }
        }
    }
}
