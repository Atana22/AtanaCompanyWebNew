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
    public class CustomersController : Controller
    {
        private readonly TEST_DOOContext _context;

        public CustomersController(TEST_DOOContext context)
        {
            _context = context;
        }

        // GET: Customers
        [Authorize(Roles = "Administrator, User")]
        public async Task<IActionResult> Index(int? pageNumber, string searchString)
        {
            int pageSize = 10;

            var tEST_DOOContext = _context.Customers.AsQueryable();

            if (!String.IsNullOrEmpty(searchString))
            {

                int prefixLength = searchString.ToString().Length;

                tEST_DOOContext = tEST_DOOContext.Where(o => o.Custid.ToString().StartsWith(searchString.ToString().Substring(0, prefixLength))
                                                       || o.Companyname.Contains(searchString)
                                                       || o.Contactname.Contains(searchString)
                                                       || o.City.Contains(searchString)
                                                       || o.Phone.Contains(searchString)
                                                       || o.Country.Contains(searchString));
            }

            tEST_DOOContext = tEST_DOOContext.OrderByDescending(o => o.Custid);

            var paginatedList = PaginatedList<Customer>.Create(await tEST_DOOContext.ToListAsync(), pageNumber ?? 1, pageSize);

            ViewData["SearchString"] = searchString;

            return View(paginatedList);
        }

        public IActionResult ExcelExport()
        {
            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("Customers");
                var currentRow = 1;

                #region Header
                worksheet.Cell(currentRow, 1).Value = "Cust ID";
                worksheet.Cell(currentRow, 2).Value = "Companyname";
                worksheet.Cell(currentRow, 3).Value = "Contactname";
                worksheet.Cell(currentRow, 4).Value = "Contacttitle";
                worksheet.Cell(currentRow, 5).Value = "Address";
                worksheet.Cell(currentRow, 6).Value = "City";
                worksheet.Cell(currentRow, 7).Value = "Region";
                worksheet.Cell(currentRow, 8).Value = "Country";
                worksheet.Cell(currentRow, 9).Value = "Phone";
                worksheet.Cell(currentRow, 10).Value = "Fax";
                #endregion

                #region Body

                foreach (var customer in _context.Customers)
                {
                    currentRow++;
                    worksheet.Cell(currentRow, 1).Value = customer.Custid;
                    worksheet.Cell(currentRow, 2).Value = customer.Companyname;
                    worksheet.Cell(currentRow, 3).Value = customer.Contactname;
                    worksheet.Cell(currentRow, 4).Value = customer.Contacttitle;
                    worksheet.Cell(currentRow, 5).Value = customer.Address;
                    worksheet.Cell(currentRow, 6).Value = customer.City;
                    worksheet.Cell(currentRow, 7).Value = customer.Region;
                    worksheet.Cell(currentRow, 8).Value = customer.Country;
                    worksheet.Cell(currentRow, 9).Value = customer.Phone;
                    worksheet.Cell(currentRow, 10).Value = customer.Fax;

                }
                #endregion

                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    var content = stream.ToArray();

                    return File(
                        content,
                        "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                        "Customers.xlsx"
                        );
                }
            }
        }

        // GET: Customers/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Customers == null)
            {
                return NotFound();
            }

            var customer = await _context.Customers
                .FirstOrDefaultAsync(m => m.Custid == id);
            if (customer == null)
            {
                return NotFound();
            }

            return View(customer);
        }

        // GET: Customers/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Customers/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Custid,Companyname,Contactname,Contacttitle,Address,City,Region,Postalcode,Country,Phone,Fax")] Customer customer)
        {
            if (ModelState.IsValid)
            {
                _context.Add(customer);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(customer);
        }

        // GET: Customers/Edit/5
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Customers == null)
            {
                return NotFound();
            }

            var customer = await _context.Customers.FindAsync(id);
            if (customer == null)
            {
                return NotFound();
            }
            return View(customer);
        }

        // POST: Customers/Edit/5

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> Edit(int id, [Bind("Custid,Companyname,Contactname,Contacttitle,Address,City,Region,Postalcode,Country,Phone,Fax")] Customer customer)
        {
            if (id != customer.Custid)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(customer);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CustomerExists(customer.Custid))
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
            return View(customer);
        }

        // GET: Customers/Delete/5
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Customers == null)
            {
                return NotFound();
            }

            var customer = await _context.Customers
                .FirstOrDefaultAsync(m => m.Custid == id);
            if (customer == null)
            {
                return NotFound();
            }

            return View(customer);
        }

        // POST: Customers/Delete/5
        [Authorize(Roles = "Administrator")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Customers == null)
            {
                return Problem("Entity set 'TEST_DOOContext.Customers'  is null.");
            }
            var customer = await _context.Customers.FindAsync(id);
            if (customer != null)
            {
                _context.Customers.Remove(customer);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CustomerExists(int id)
        {
          return (_context.Customers?.Any(e => e.Custid == id)).GetValueOrDefault();
        }
    }
}
