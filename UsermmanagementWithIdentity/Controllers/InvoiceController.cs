using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using UsermmanagementWithIdentity.Data;
using UsermmanagementWithIdentity.Models;

namespace UsermmanagementWithIdentity.Controllers
{
    public class InvoiceController : Controller
    {
        private readonly ApplicationDbContext _context;
        public InvoiceController(ApplicationDbContext context)
        { 
            _context = context;
        }
        public IActionResult Index()
        {
            var invoices = _context.Invoices.ToList();
            return View(invoices);
        }
        [HttpGet]
        public IActionResult Add()
        {
            ViewBag.listProduct=new SelectList(_context.Products.ToList(),"Id", "ProductName");
            return View(new Invoice());
        }
        [HttpGet]
        public IActionResult GetProductDetails(int id)
        {
            var product = _context.Products.FirstOrDefault(p => p.Id == id);

            if (product == null)
            {
                return Json(null);
            }

            return Json(new
            {
                name = product.ProductName,
                price = product.Price
            });
        }
        [HttpPost]
        public IActionResult Add(Invoice invoice)
        {
            if (!ModelState.IsValid)
            {
                return View(invoice);   
            }
            invoice.DateOfCreation =  DateTime.Now;
            _context.Invoices.Add(invoice); 
            _context.SaveChanges();
            return RedirectToAction("Index");
        }
        [HttpGet]
        public IActionResult GenerateNewCode()
        {
            // Generate a new code based on the last record in the database
            var lastProduct = _context.Invoices.OrderByDescending(p => p.Code).FirstOrDefault();

            int newCodeNumber = lastProduct != null ? lastProduct.Code+ 1 : 1; // Start from 1001 if no records exist

            return Json(new { code = newCodeNumber });
        }
        [HttpGet]
        public IActionResult GetInvoiceDetails(int id)
        {
            var invoice = _context.Invoices.FirstOrDefault(i => i.Id == id);

            if (invoice == null)
            {
                return NotFound();
            }

            return Json(new
            {
                code = invoice.Code,
                customerName = invoice.CustomerName,
                productName = invoice.ProductName,
                price = invoice.Price,
                creationDate = invoice.DateOfCreation.ToString("yyyy-MM-dd"), // Adjust date format if needed
                registrationDate = invoice.DateOfRegistration.ToString("yyyy-MM-dd")
            });
        }
        public IActionResult Print(int id)
        {
            var invoice = _context.Invoices.FirstOrDefault(i => i.Id == id);
            if (invoice == null)
            {
                return NotFound();
            }
            return View(invoice); // Pass invoice to a new Print.cshtml view
        }

    }
}
