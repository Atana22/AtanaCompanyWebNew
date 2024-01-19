using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using AtanaCompanyWeb.Models;
using Microsoft.AspNetCore.Authorization;
using ClosedXML.Excel;

namespace AtanaCompanyWeb.Controllers
{
    public class OrderDetailsController : Controller
    {
        private readonly TEST_DOOContext _context;

        public OrderDetailsController(TEST_DOOContext context)
        {
            _context = context;
        }

        // GET: OrderDetails
        [Authorize(Roles = "Administrator, User")]
        public async Task<IActionResult> Index(int? pageNumber, string searchString)
        {
            int pageSize = 10;

            var tEST_DOOContext = _context.OrderDetails.Include(o => o.Order).Include(o => o.Product).AsQueryable();

            if (!String.IsNullOrEmpty(searchString))
            {

                int prefixLength = searchString.ToString().Length;

                tEST_DOOContext = tEST_DOOContext.Where(o => o.Orderid.ToString().StartsWith(searchString.ToString().Substring(0, prefixLength)));
            }

            tEST_DOOContext = tEST_DOOContext.OrderByDescending(o => o.Orderid);

            var paginatedList = PaginatedList<OrderDetail>.Create(await tEST_DOOContext.ToListAsync(), pageNumber ?? 1, pageSize);

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
                worksheet.Cell(currentRow, 2).Value = "Product ID";
                worksheet.Cell(currentRow, 3).Value = "Unitprice";
                worksheet.Cell(currentRow, 4).Value = "Qty";
                worksheet.Cell(currentRow, 5).Value = "Discount";
                worksheet.Cell(currentRow, 6).Value = "TotalValue";
                #endregion

                #region Body

                foreach (var orderDetails in _context.OrderDetails)
                {
                    currentRow++;
                    worksheet.Cell(currentRow, 1).Value = orderDetails.Orderid;
                    worksheet.Cell(currentRow, 2).Value = orderDetails.Productid;
                    worksheet.Cell(currentRow, 3).Value = orderDetails.Unitprice;
                    worksheet.Cell(currentRow, 4).Value = orderDetails.Qty;
                    worksheet.Cell(currentRow, 5).Value = orderDetails.Discount;
                    worksheet.Cell(currentRow, 6).Value = orderDetails.TotalValue;
                }
                #endregion

                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    var content = stream.ToArray();

                    return File(
                        content,
                        "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                        "OrderDetails.xlsx"
                        );
                }
            }
        }

        // GET: OrderDetails/Create
        public IActionResult Create()
        {
            ViewData["Orderid"] = new SelectList(_context.Orders, "Orderid", "Orderid");
            ViewData["Productid"] = new SelectList(_context.Products, "Productid", "Productid");
            return View();
        }


        // POST: OrderDetails/Create

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Orderid,Productid,Unitprice,Qty,Discount")] OrderDetail orderDetail)
        {
            ModelState.Remove("Order");
            ModelState.Remove("Product");//izbacio sam ih iz modela za proveru, jer su suvisna

            if (ModelState.IsValid)
            {
                _context.Add(orderDetail);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["Orderid"] = new SelectList(_context.Orders, "Orderid", "Orderid", orderDetail.Orderid);
            ViewData["Productid"] = new SelectList(_context.Products, "Productid", "Productid", orderDetail.Productid);
            return View(orderDetail);
        }

        // GET: OrderDetails/Edit/5
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.OrderDetails == null)
            {
                return NotFound();
            }

            //var orderDetail = await _context.OrderDetails.FindAsync(id);

            var orderDetail = await _context.OrderDetails
            .Where(od => od.Orderid == id) 
            .FirstOrDefaultAsync();

            if (orderDetail == null)
            {
                return NotFound();
            }
            ViewData["Orderid"] = new SelectList(_context.Orders, "Orderid", "Orderid", orderDetail.Orderid);
            ViewData["Productid"] = new SelectList(_context.Products, "Productid", "Productid", orderDetail.Productid);

            return View(orderDetail);
        }
        
        // POST: OrderDetails/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> Edit(int id, [Bind("Orderid,Productid,Unitprice,Qty,Discount")] OrderDetail orderDetail)
        {
            if (id != orderDetail.Orderid)
            {
                return NotFound();
            }

            ModelState.Remove("Order");
            ModelState.Remove("Product");

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(orderDetail);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!OrderDetailExists(orderDetail.Orderid))
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
            ViewData["Orderid"] = new SelectList(_context.Orders, "Orderid", "Orderid", orderDetail.Orderid);
            ViewData["Productid"] = new SelectList(_context.Products, "Productid", "Productid", orderDetail.Productid);
            return View(orderDetail);
        }

        // GET: OrderDetails/Delete/5
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.OrderDetails == null)
            {
                return NotFound();
            }

            var orderDetail = await _context.OrderDetails
                .Include(o => o.Order)
                .Include(o => o.Product)
                .FirstOrDefaultAsync(m => m.Orderid == id);
            if (orderDetail == null)
            {
                return NotFound();
            }

            return View(orderDetail);
        }

        [HttpPost, ActionName("DeleteConfirmed")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> DeleteConfirmed(int orderId, int productId)
        {
            if (_context.OrderDetails == null)
            {
                return Problem("Entity set 'TEST_DOOContext.OrderDetails' is null.");
            }

            var orderDetail = await _context.OrderDetails.FindAsync(orderId, productId); //salje dva ulazna parametra da bi znao tacno koji je orderDetails zeli da izbrise

            if (orderDetail != null)
            {
                _context.OrderDetails.Remove(orderDetail);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }


        private bool OrderDetailExists(int id)
        {
          return (_context.OrderDetails?.Any(e => e.Orderid == id)).GetValueOrDefault();
        }
    }
}
