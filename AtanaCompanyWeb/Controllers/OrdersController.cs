using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using AtanaCompanyWeb.Models;
using NuGet.Protocol.Core.Types;
using Microsoft.AspNetCore.Authorization;
using ClosedXML.Excel;


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
        [Authorize(Roles = "Administrator, User")]
        public async Task<IActionResult> Index(int? pageNumber, string searchString)
        {
            int pageSize = 25; //broj recorda linija po starni


            var tEST_DOOContext = _context.Orders.AsQueryable(); //mora sam da castujem tEST_DOOContext posto nije hteo da radi where, ako su pre njega INCLUDE

            if (!String.IsNullOrEmpty(searchString)) 
            {

                int prefixLength = searchString.ToString().Length;

                tEST_DOOContext = tEST_DOOContext.Where(o => o.Orderid.ToString().StartsWith(searchString.ToString().Substring(0, prefixLength))
                                                       || o.Shipcity.Contains(searchString)
                                                       || o.Shipcountry.Contains(searchString)
                                                       || o.Orderstatus.Contains(searchString));
            }

            tEST_DOOContext = tEST_DOOContext.OrderByDescending(o => o.Orderid);
                                    

            //return View(await tEST_DOOContext.ToListAsync());

            var paginatedList = PaginatedList<Order>.Create(await tEST_DOOContext.ToListAsync(), pageNumber ?? 1, pageSize);

            ViewData["SearchString"] = searchString;
         

            return View(paginatedList);
        }

        public IActionResult ExcelExport()
        {
            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("Orders");
                var currentRow = 1;

                #region Header
                worksheet.Cell(currentRow, 1).Value = "Order ID";
                worksheet.Cell(currentRow, 2).Value = "Orderdate";
                worksheet.Cell(currentRow, 3).Value = "Requireddate";
                worksheet.Cell(currentRow, 4).Value = "Shippeddate";
                worksheet.Cell(currentRow, 5).Value = "Freight";
                worksheet.Cell(currentRow, 6).Value = "Shipname";
                worksheet.Cell(currentRow, 7).Value = "Shipaddress";
                worksheet.Cell(currentRow, 8).Value = "Shipcity";
                worksheet.Cell(currentRow, 9).Value = "Shippostalcode";
                worksheet.Cell(currentRow, 10).Value = "Shipcountry";
                worksheet.Cell(currentRow, 11).Value = "Orderstatus";
                #endregion

                #region Body

                foreach (var orders in _context.Orders)
                {
                    currentRow++;
                    worksheet.Cell(currentRow, 1).Value = orders.Orderid;
                    worksheet.Cell(currentRow, 2).Value = orders.Orderdate;
                    worksheet.Cell(currentRow, 3).Value = orders.Requireddate;
                    worksheet.Cell(currentRow, 4).Value = orders.Shippeddate;
                    worksheet.Cell(currentRow, 5).Value = orders.Freight;
                    worksheet.Cell(currentRow, 6).Value = orders.Shipname;
                    worksheet.Cell(currentRow, 7).Value = orders.Shipaddress;
                    worksheet.Cell(currentRow, 8).Value = orders.Shipcity;
                    worksheet.Cell(currentRow, 9).Value = orders.Shippostalcode;
                    worksheet.Cell(currentRow, 10).Value = orders.Shipcountry;
                    worksheet.Cell(currentRow, 11).Value = orders.Orderstatus;
                }
                #endregion

                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    var content = stream.ToArray();

                    return File(
                        content,
                        "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                        "Orders.xlsx"
                        );
                }
            }
        }

        // GET: Orders/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Orders == null)
            {
                return NotFound();
            }

            //var order = await _context.Orders
            //     .Include(o => o.OrderDetails)
            //      .FirstOrDefaultAsync(m => m.Orderid == id);

            var order = await _context.Orders
                .Include(o => o.Cust)  //we use the include method to combine the related entities in the same query, 
                .Include(o => o.Emp)   //like merging multiple entities in a single query. https://www.educba.com/linq-include/
                .Include(o => o.Shipper)
                .Include(o => o.OrderDetails).FirstOrDefaultAsync(m => m.Orderid == id);
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

            ViewData["Orderstatus"] = new SelectList(_context.Orders.Select(o => o.Orderstatus).Distinct().ToList());

            return View();
        }

        // POST: Orders/Create

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Orderid,Custid,Empid,Orderdate,Requireddate,Shippeddate,Shipperid,Freight,Shipname,Shipaddress,Shipcity,Shipregion,Shippostalcode,Shipcountry,Orderstatus")] Order order)
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

            ViewData["Orderstatus"] = new SelectList(_context.Orders.Select(o => o.Orderstatus).Distinct().ToList());

            return View(order);
        }

        // GET: Orders/Edit/5

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Orders == null)
            {
                return NotFound();
            }

            var order = await _context.Orders.FindAsync(id); //Find trazu vrednost na osnovu Primery Key, a ovde je Orderid je PK pa moze Find 
            //var orderFromDbFirst = _context.Orders.FirstOrDefaultAsync(u => u.Orderid == id);
            //var orderFromDbSingle = _context.Orders.SingleOrDefaultAsync(u => u.Orderid == id);

            if (order == null)
            {
                return NotFound();
            }

            ViewData["Custid"] = new SelectList(_context.Customers, "Custid", "Custid", order.Custid);
            ViewData["Empid"] = new SelectList(_context.Employees, "Empid", "Empid", order.Empid);
            ViewData["Shipperid"] = new SelectList(_context.Shippers, "Shipperid", "Shipperid", order.Shipperid);

            ViewData["Orderstatus"] = new SelectList(_context.Orders.Select(o => o.Orderstatus).Distinct().ToList());

            return View(order);
        }

        // POST: Orders/Edit/5

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Orderid,Custid,Empid,Orderdate,Requireddate,Shippeddate,Shipperid,Freight,Shipname,Shipaddress,Shipcity,Shipregion,Shippostalcode,Shipcountry,Orderstatus")] Order order)
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
            ViewData["Orderstatus"] = new SelectList(_context.Orders.Select(o => o.Orderstatus).Distinct().ToList());

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
