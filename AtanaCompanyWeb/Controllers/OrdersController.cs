using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using AtanaCompanyWeb.Models;

namespace AtanaCompanyWeb.Controllers
{
    public class OrdersController : Controller
    {
        private readonly TEST_DOOContext _context;

        public OrdersController(TEST_DOOContext context)
        {
            _context = context;
        }

        // GET: Orders
        public async Task<IActionResult> Index()
        {
            var tEST_DOOContext = _context.Orders.Include(o => o.Cust).Include(o => o.Emp).Include(o => o.Shipper);
            return View(await tEST_DOOContext.ToListAsync());
        }

        // GET: Orders/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Orders == null)
            {
                return NotFound();
            }

            var order = await _context.Orders
                .Include(o => o.Cust)  //we use the include method to combine the related entities in the same query, 
                .Include(o => o.Emp)   //like merging multiple entities in a single query. https://www.educba.com/linq-include/
                .Include(o => o.Shipper)
                .FirstOrDefaultAsync(m => m.Orderid == id);
            if (order == null)
            {
                return NotFound(); // return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            return View(order);
        }

        // GET: Orders/Create
        public IActionResult Create()
        {
            ViewData["Custid"] = new SelectList(_context.Customers, "Custid", "Custid");
            ViewData["Empid"] = new SelectList(_context.Employees, "Empid", "Empid");
            ViewData["Shipperid"] = new SelectList(_context.Shippers, "Shipperid", "Shipperid");
            return View();
        }

        // POST: Orders/Create

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Orderid,Custid,Empid,Orderdate,Requireddate,Shippeddate,Shipperid,Freight,Shipname,Shipaddress,Shipcity,Shipregion,Shippostalcode,Shipcountry")] Order order)
        {

            if (order.Orderdate > order.Shippeddate) //custom error
            {
                ModelState.AddModelError("CustomError", "The Shipping date can not be higer then the Order day");
            }

            ModelState.Remove("Emp"); //obrisao sam iz ModelState, posto se ove vrednosti ne popunjavaju u ovoj formi
            ModelState.Remove("Shipper");

            if (ModelState.IsValid)
            {
                _context.Add(order);
                await _context.SaveChangesAsync();
                TempData["success"] = "Order created successully!";
                return RedirectToAction(nameof(Index));
            }
            ViewData["Custid"] = new SelectList(_context.Customers, "Custid", "Custid", order.Custid); // SelectList je za dropdown vrednosti 
            ViewData["Empid"] = new SelectList(_context.Employees, "Empid", "Empid", order.Empid);
            ViewData["Shipperid"] = new SelectList(_context.Shippers, "Shipperid", "Shipperid", order.Shipperid);

            return View(order);
        }

        // GET: Orders/Edit/5

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Orders == null)
            {
                return NotFound();
            }

            var order = await _context.Orders.FindAsync(id); //Find trazu vrednost na osnovu Primery Key, a ovde je Orderid je PK pa moze Find - https://github.com/dotnet/AspNetCore.Docs/issues/15498
            //var orderFromDbFirst = _context.Orders.FirstOrDefaultAsync(u => u.Orderid == id);
            //var orderFromDbSingle = _context.Orders.SingleOrDefaultAsync(u => u.Orderid == id);


            if (order == null)
            {
                return NotFound();
            }

            ViewData["Custid"] = new SelectList(_context.Customers, "Custid", "Custid", order.Custid);
            ViewData["Empid"] = new SelectList(_context.Employees, "Empid", "Empid", order.Empid);
            ViewData["Shipperid"] = new SelectList(_context.Shippers, "Shipperid", "Shipperid", order.Shipperid);

            return View(order);
        }

        // POST: Orders/Edit/5

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Orderid,Custid,Empid,Orderdate,Requireddate,Shippeddate,Shipperid,Freight,Shipname,Shipaddress,Shipcity,Shipregion,Shippostalcode,Shipcountry")] Order order)
        {
            if (id != order.Orderid)
            {
                return NotFound();
            }

            ModelState.Remove("Emp");
            ModelState.Remove("Shipper");

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(order);
                    await _context.SaveChangesAsync();
                    TempData["success"] = "Order updared successully!";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!OrderExists(order.Orderid))
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

            ViewData["Custid"] = new SelectList(_context.Customers, "Custid", "Custid", order.Custid);
            ViewData["Empid"] = new SelectList(_context.Employees, "Empid", "Empid", order.Empid);
            ViewData["Shipperid"] = new SelectList(_context.Shippers, "Shipperid", "Shipperid", order.Shipperid);

            return View(order);
        }

        // GET: Orders/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Orders == null)
            {
                return NotFound();
            }

            var order = await _context.Orders
                .Include(o => o.Cust)
                .Include(o => o.Emp)
                .Include(o => o.Shipper)
                .FirstOrDefaultAsync(m => m.Orderid == id);
            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }

        // POST: Orders/Delete/5
        [HttpPost, ActionName("Delete")] // u View akcije se zove DELTE(<form asp-action="Delete">), a kod metoda se zove DeleteConfirmed, nece je preoznati
        [ValidateAntiForgeryToken]       // tako da preko ActionName("Delete") preminujemo akciju
        public async Task<IActionResult> DeleteConfirmed(int id) //ne moze Delete, mora drugi naziv (DeleteConfirmed)
        {
            if (_context.Orders == null)
            {
                return Problem("Entity set 'TEST_DOOContext.Orders'  is null.");
            }
            var order = await _context.Orders.FindAsync(id);
            if (order != null)
            {
                _context.Orders.Remove(order);
            }

            await _context.SaveChangesAsync();
            TempData["success"] = "Order deleted successully!";
            return RedirectToAction(nameof(Index));
        }

        private bool OrderExists(int id)
        {
            return (_context.Orders?.Any(e => e.Orderid == id)).GetValueOrDefault();
        }
    }
}
