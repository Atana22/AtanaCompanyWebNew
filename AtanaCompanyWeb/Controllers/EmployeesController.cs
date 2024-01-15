﻿    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Rendering;
    using Microsoft.EntityFrameworkCore;
    using AtanaCompanyWeb.Models;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

    namespace AtanaCompanyWeb.Controllers
    {
        public class EmployeesController : Controller
        {
            private readonly TEST_DOOContext _context;

            public EmployeesController(TEST_DOOContext context)
            {
                _context = context;
            }

        // GET: Employees
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> Index()
            {
                var tEST_DOOContext = _context.Employees;
                return View(await tEST_DOOContext.ToListAsync());
            }

            [HttpGet]
            public IActionResult Login()
            {
                return View();
            }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var employee = await _context.Employees
                    .FirstOrDefaultAsync(e => e.Login == model.Login && e.Password == model.Password);

                if (employee != null)
                {
                    var roles = new List<Claim>
            {
                new Claim(ClaimTypes.Name, employee.Login)
            };

                    if (employee.Roles.Contains("Administrator"))
                    {
                        roles.Add(new Claim(ClaimTypes.Role, "Administrator"));
                    }

                    if (employee.Roles.Contains("User"))
                    {
                        roles.Add(new Claim(ClaimTypes.Role, "User"));
                    }

                    var identity = new ClaimsIdentity(roles, "login");
                    var principal = new ClaimsPrincipal(identity);

                    await HttpContext.SignInAsync(principal);

                    return RedirectToAction("Index", "Home");
                }

                ModelState.AddModelError(string.Empty, "Invalid login attempt");
            }

            return View(model);
        }


        [HttpGet]
            public IActionResult ForgotPassword()
            {
                return View();
            }

            [HttpPost]
            [ValidateAntiForgeryToken]
            public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel model)
            {
                if (ModelState.IsValid)
                {
                    var user = await _context.Employees.FirstOrDefaultAsync(e => e.Email == model.Email && e.City == model.City);

                    if (user != null)
                    {
                        return RedirectToAction("ResetPassword", "Employees");
                    }

                    ModelState.AddModelError(string.Empty, "The data do not exist in the database. Try again!");
                }

                return View(model);
            }


            [HttpGet]
            public IActionResult ResetPassword()
            {
                return View();
            }

            [HttpPost]
            [ValidateAntiForgeryToken]
            public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
            {
                if (ModelState.IsValid)
                {
                    try
                    {
                        var employee = await _context.Employees
                            .FirstOrDefaultAsync(e => e.Email == model.Email);

                        if (employee != null)
                        {
                            employee.Password = model.NewPassword;

                            await _context.SaveChangesAsync();

                            return RedirectToAction("Login");
                        }
                        else
                        {
                            ModelState.AddModelError(string.Empty, "User not found.");
                        }
                    }
                    catch (Exception ex)
                    {
                        ModelState.AddModelError(string.Empty, $"An error occurred: {ex.Message}");
                    }
                }

                return View(model);
            }



            // GET: Employees/Details/5
            public async Task<IActionResult> Details(int? id)
            {
                if (id == null || _context.Employees == null)
                {
                    return NotFound();
                }

                var employee = await _context.Employees
                    .Include(e => e.Mgr)
                    .FirstOrDefaultAsync(m => m.Empid == id);
                if (employee == null)
                {
                    return NotFound();
                }

                return View(employee);
            }

            // GET: Employees/Create
            public IActionResult Create()
            {
                ViewData["Mgrid"] = new SelectList(_context.Employees, "Empid", "Empid");
                return View();
            }

            // POST: Employees/Create
 
            [HttpPost]
            [ValidateAntiForgeryToken]
            public async Task<IActionResult> Create([Bind("Empid,Lastname,Firstname,Title,Titleofcourtesy,Birthdate,Hiredate,Address,City,Region,Postalcode,Country,Email,Roles,Mgrid,Login,Password")] Employee employee)
            {
                if (ModelState.IsValid)
                {
                    _context.Add(employee);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                ViewData["Mgrid"] = new SelectList(_context.Employees, "Empid", "Empid", employee.Mgrid);
                return View(employee);
            }

            // GET: Employees/Edit/5
            public async Task<IActionResult> Edit(int? id)
            {
                if (id == null || _context.Employees == null)
                {
                    return NotFound();
                }

                var employee = await _context.Employees.FindAsync(id);
                if (employee == null)
                {
                    return NotFound();
                }
                ViewData["Mgrid"] = new SelectList(_context.Employees, "Empid", "Empid", employee.Mgrid);
                return View(employee);
            }

            // POST: Employees/Edit/5

            [HttpPost]
            [ValidateAntiForgeryToken]
            public async Task<IActionResult> Edit(int id, [Bind("Empid,Lastname,Firstname,Title,Titleofcourtesy,Birthdate,Hiredate,Address,City,Region,Postalcode,Country,Email,Roles,Mgrid,Login,Password")] Employee employee)
            {
                if (id != employee.Empid)
                {
                    return NotFound();
                }

                if (ModelState.IsValid)
                {
                    try
                    {
                        _context.Update(employee);
                        await _context.SaveChangesAsync();
                    }
                    catch (DbUpdateConcurrencyException)
                    {
                        if (!EmployeeExists(employee.Empid))
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
                ViewData["Mgrid"] = new SelectList(_context.Employees, "Empid", "Empid", employee.Mgrid);
                return View(employee);
            }

            // GET: Employees/Delete/5
            public async Task<IActionResult> Delete(int? id)
            {
                if (id == null || _context.Employees == null)
                {
                    return NotFound();
                }

                var employee = await _context.Employees
                    .Include(e => e.Mgr)
                    .FirstOrDefaultAsync(m => m.Empid == id);
                if (employee == null)
                {
                    return NotFound();
                }

                return View(employee);
            }

            // POST: Employees/Delete/5
            [HttpPost, ActionName("Delete")]
            [ValidateAntiForgeryToken]
            public async Task<IActionResult> DeleteConfirmed(int id)
            {
                if (_context.Employees == null)
                {
                    return Problem("Entity set 'TEST_DOOContext.Employees'  is null.");
                }
                var employee = await _context.Employees.FindAsync(id);
                if (employee != null)
                {
                    _context.Employees.Remove(employee);
                }
            
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            private bool EmployeeExists(int id)
            {
              return (_context.Employees?.Any(e => e.Empid == id)).GetValueOrDefault();
            }
        }
    }
