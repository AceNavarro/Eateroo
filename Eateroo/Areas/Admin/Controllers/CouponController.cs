using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Eateroo.Data;
using Eateroo.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Eateroo.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class CouponController : Controller
    {
        private readonly ApplicationDbContext _db;

        public CouponController(ApplicationDbContext db)
        {
            _db = db;
        }

        [BindProperty]
        public Coupon Coupon { get; set; }

        // INDEX --------------------------------------------------------------
        // GET
        public async Task<IActionResult> Index()
        {
            return View(await _db.Coupon.ToListAsync());
        }

        // CREATE -------------------------------------------------------------
        // GET
        public IActionResult Create()
        {
            Coupon = new Coupon();
            return View(Coupon);
        }

        // POST
        [HttpPost, ActionName("Create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> PostCreate()
        {
            if (!ModelState.IsValid)
            {
                return View(Coupon);
            }

            var files = Request.Form.Files;

            if (files.Any())
            {
                byte[] imageData = null;

                using (var fs = files[0].OpenReadStream())
                {
                    using (var ms = new MemoryStream())
                    {
                        await fs.CopyToAsync(ms);
                        imageData = ms.ToArray();
                    }
                }

                Coupon.Image = imageData;
            }

            _db.Coupon.Add(Coupon);
            await _db.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        // EDIT ---------------------------------------------------------------
        // GET
        public async Task<IActionResult> Edit(int id)
        {
            Coupon = await _db.Coupon.FindAsync(id);
            if (Coupon == null)
            {
                return NotFound();
            }

            return View(Coupon);
        }

        // POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit()
        {
            if (!ModelState.IsValid)
            {
                return View(Coupon);
            }

            var couponFromDb = await _db.Coupon.FindAsync(Coupon.Id);
            if (couponFromDb == null)
            {
                return NotFound();
            }

            var files = Request.Form.Files;

            if (files.Any())
            {
                byte[] imageData = null;

                using (var fs = files[0].OpenReadStream())
                {
                    using (var ms = new MemoryStream())
                    {
                        await fs.CopyToAsync(ms);
                        imageData = ms.ToArray();
                    }
                }

                couponFromDb.Image = imageData;
            }

            couponFromDb.Name = Coupon.Name;
            couponFromDb.CouponType = Coupon.CouponType;
            couponFromDb.Discount = Coupon.Discount;
            couponFromDb.MinimumAmount = Coupon.MinimumAmount;
            couponFromDb.IsActive = Coupon.IsActive;
            
            await _db.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        // DETAIL -------------------------------------------------------------
        // GET
        public async Task<IActionResult> Detail(int id)
        {
            Coupon = await _db.Coupon.FindAsync(id);
            if (Coupon == null)
            {
                return NotFound();
            }

            return View(Coupon);
        }

        // DELETE -------------------------------------------------------------
        // POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var couponFromDb = await _db.Coupon.FindAsync(id);
            if (couponFromDb == null)
            {
                return NotFound();
            }

            _db.Coupon.Remove(couponFromDb);
            await _db.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
    }
}