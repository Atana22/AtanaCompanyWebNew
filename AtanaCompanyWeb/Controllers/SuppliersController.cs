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
    public class SuppliersController : Controller
    {
        private readonly TEST_DOOContext _context;

        public SuppliersController(TEST_DOOContext context)
        {
            _context = context;
        }

        // GET: Suppliers
        [Authorize(Roles = "Administrator, User")]
        public async Task<IActionResult> Index(int? pageNumber, string searchString)
        {
            int pageSize = 10;

            var tEST_DOOContext = _context.Suppliers.AsQueryable();

            if (!String.IsNullOrEmpty(searchString))
            {

                int prefixLength = searchString.ToString().Length;

                tEST_DOOContext = tEST_DOOContext.Where(o => o.Supplierid.ToString().StartsWith(searchString.ToString().Substring(0, prefixLength))
                                                   || o.Companyname.Contains(searchString)
                                                   || o.Contactname.Contains(searchString)
                                                   || o.City.Contains(searchString)
                                                   || o.Phone.Contains(searchString)
                                                   || o.Country.Contains(searchString));
            }

            tEST_DOOContext = tEST_DOOContext.OrderByDescending(o => o.Supplierid);

            var paginatedList = PaginatedList<Supplier>.Create(await tEST_DOOContext.ToListAsync(), pageNumber ?? 1, pageSize);

            ViewData["SearchString"] = searchString;

            return View(paginatedList);
        }

        public IActionResult ExcelExport()
        {
            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("Suppliers");
                var currentRow = 1;

                #region Header
                worksheet.Cell(currentRow, 1).Value = "Suppliers ID";
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

                foreach (var supplier in _context.Suppliers)
                {
                    currentRow++;
                    worksheet.Cell(currentRow, 1).Value = supplier.Supplierid;
                    worksheet.Cell(currentRow, 2).Value = supplier.Companyname;
                    worksheet.Cell(currentRow, 3).Value = supplier.Contactname;
                    worksheet.Cell(currentRow, 4).Value = supplier.Contacttitle;
                    worksheet.Cell(currentRow, 5).Value = supplier.Address;
                    worksheet.Cell(currentRow, 6).Value = supplier.City;
                    worksheet.Cell(currentRow, 7).Value = supplier.Region;
                    worksheet.Cell(currentRow, 8).Value = supplier.Country;
                    worksheet.Cell(currentRow, 9).Value = supplier.Phone;
                    worksheet.Cell(currentRow, 10).Value = supplier.Fax;

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

        // GET: Suppliers/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Suppliers == null)
            {
                return NotFound();
            }

            var supplier = await _context.Suppliers
                .FirstOrDefaultAsync(m => m.Supplierid == id);
            if (supplier == null)
            {
                return NotFound();
            }

            return View(supplier);
        }

        // GET: Suppliers/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Suppliers/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Supplierid,Companyname,Contactname,Contacttitle,Address,City,Region,Postalcode,Country,Phone,Fax")] Supplier supplier)
        {
            if (ModelState.IsValid)
            {
                _context.Add(supplier);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(supplier);
        }

        // GET: Suppliers/Edit/5
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Suppliers == null)
            {
                return NotFound();
            }

            var supplier = await _context.Suppliers.FindAsync(id);
            if (supplier == null)
            {
                return NotFound();
            }
            return View(supplier);
        }

        // POST: Suppliers/Edit/5

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> Edit(int id, [Bind("Supplierid,Companyname,Contactname,Contacttitle,Address,City,Region,Postalcode,Country,Phone,Fax")] Supplier supplier)
        {
            if (id != supplier.Supplierid)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(supplier);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SupplierExists(supplier.Supplierid))
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
            return View(supplier);
        }

        // GET: Suppliers/Delete/5
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Suppliers == null)
            {
                return NotFound();
            }

            var supplier = await _context.Suppliers
                .FirstOrDefaultAsync(m => m.Supplierid == id);
            if (supplier == null)
            {
                return NotFound();
            }

            return View(supplier);
        }

        // POST: Suppliers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Suppliers == null)
            {
                return Problem("Entity set 'TEST_DOOContext.Suppliers'  is null.");
            }
            var supplier = await _context.Suppliers.FindAsync(id);
            if (supplier != null)
            {
                _context.Suppliers.Remove(supplier);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool SupplierExists(int id)
        {
          return (_context.Suppliers?.Any(e => e.Supplierid == id)).GetValueOrDefault();
        }
    }
}
