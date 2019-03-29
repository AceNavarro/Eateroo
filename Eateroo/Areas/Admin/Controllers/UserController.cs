using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Eateroo.Data;
using Eateroo.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Eateroo.Areas.Admin.Controllers
{
    [Authorize(Roles = StaticDetail.ManagerUser)]
    [Area("Admin")]
    public class UserController : Controller
    {
        private readonly ApplicationDbContext _db;

        public UserController(ApplicationDbContext db)
        {
            _db = db;
        }

        // INDEX --------------------------------------------------------------
        // GET
        public async Task<IActionResult> Index()
        {
            // Get the currently logged-in user
            var claimsIdentity = (ClaimsIdentity)this.User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            if (claim == null)
            {
                return Redirect("/Identity/Account/Login");
            }

            // Pass-in the list of user except the currently logged-in user
            return View(await _db.ApplicationUser.Where(u => u.Id != claim.Value).ToListAsync());
        }

        // LOCK ---------------------------------------------------------------
        // POST
        public async Task<IActionResult> Lock(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return NotFound();
            }

            var user = await _db.ApplicationUser.FindAsync(id);

            if (user == null)
            {
                return NotFound();
            }

            user.LockoutEnd = DateTime.Now.AddYears(1000);
            await _db.SaveChangesAsync();
            
            return RedirectToAction(nameof(Index));
        }

        // UNLOCK -------------------------------------------------------------
        // POST
        public async Task<IActionResult> Unlock(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return NotFound();
            }

            var user = await _db.ApplicationUser.FindAsync(id);

            if (user == null)
            {
                return NotFound();
            }

            user.LockoutEnd = null;
            await _db.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
    }
}