using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Eateroo.Data;
using Eateroo.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Eateroo.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class CategoryController : Controller
    {
        private readonly ApplicationDbContext _db;

        public CategoryController(ApplicationDbContext db)
        {
            _db = db;
        }

        // INDEX --------------------------------------------------------------
        // GET
        public async Task<IActionResult> Index()
        {
            return View(await _db.Category.ToListAsync());
        }

        // CREATE -------------------------------------------------------------
        // GET
        public IActionResult Create()
        {
            return View();
        }

        // POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Category category)
        {
            if (category == null)
            {
                return UnprocessableEntity();
            }

            if (!ModelState.IsValid)
            {
                return View(category);
            }

            _db.Add(category);
            await _db.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        // EDIT ---------------------------------------------------------------
        // GET
        public async Task<IActionResult> Edit(int id)
        {
            var category = await _db.Category.FindAsync(id);
            if (category == null)
            {
                return NotFound();
            }

            return View(category);
        }

        // POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Category category)
        {
            if (category == null)
            {
                return UnprocessableEntity();
            }

            if (!ModelState.IsValid)
            {
                return View(category);
            }

            _db.Update(category);
            await _db.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        // DETAIL --------------------------------------------------------------
        // GET
        public async Task<IActionResult> Detail(int id)
        {
            var category = await _db.Category.FindAsync(id);
            if (category == null)
            {
                return NotFound();
            }

            return View(category);
        }

        // DELETE -------------------------------------------------------------
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var category = await _db.Category.FindAsync(id);
            if (category == null)
            {
                return NotFound();
            }

            _db.Category.Remove(category);
            await _db.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
    }
}