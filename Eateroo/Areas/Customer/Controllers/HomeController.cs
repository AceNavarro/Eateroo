using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Eateroo.Models;
using Eateroo.Models.ViewModels;
using Eateroo.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Eateroo.Utility;

namespace Eateroo.Controllers.Customer
{
    [Area("Customer")]
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _db;

        public HomeController(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<IActionResult> Index()
        {
            // Get user id
            var claimsIdentity = (ClaimsIdentity)this.User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            // Count the total number of items in the cart and store in session
            int totalCount = 0;
            if (claim != null)
            {
                totalCount = await _db.ShoppingCart.Where(c => c.ApplicationUserId == claim.Value).SumAsync(c => c.Count);
            }
            HttpContext.Session.SetInt32(StaticDetail.ShoppingCartCount, totalCount);

            // Create the view model
            var viewModel = new IndexViewModel()
            {
                MenuItems = await _db.MenuItem.Include(m => m.Category)
                                              .Include(m => m.SubCategory)
                                              .ToListAsync(),
                Categories = await _db.Category.ToListAsync(),
                Coupons = await _db.Coupon.Where(c => c.IsActive).ToListAsync()
            };

            return View(viewModel);
        }

        [Authorize()]
        public async Task<IActionResult> Details(int id)
        {
            var menuItem = await _db.MenuItem.Include(m => m.Category)
                                             .Include(m => m.SubCategory)
                                             .FirstOrDefaultAsync(m => m.Id == id);

            if (menuItem == null)
            {
                return NotFound();
            }

            var shoppingCart = new ShoppingCart()
            {
                MenuItem = menuItem,
                MenuItemId = menuItem.Id
            };

            return View(shoppingCart);
        }

        [Authorize()]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddToCart(ShoppingCart cart)
        {
            if (cart == null)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                return View(cart);
            }

            // Get user id
            var claimsIdentity = (ClaimsIdentity)this.User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            // Set user id into cart object
            cart.ApplicationUserId = claim.Value;

            var cartFromDb = await _db.ShoppingCart.FirstOrDefaultAsync(c => c.ApplicationUserId == cart.ApplicationUserId &&
                                                                             c.MenuItemId == cart.MenuItemId);

            // Menu item on shopping cart already exist
            if (cartFromDb != null)
            {
                cartFromDb.Count = cartFromDb.Count + cart.Count;
            }
            // Menu items does not exist yet on shopping cart
            else
            {
                _db.ShoppingCart.Add(cart);
            }

            await _db.SaveChangesAsync();

            // Count the total number of items in the cart
            var totalCount = await _db.ShoppingCart.Where(c => c.ApplicationUserId == cart.ApplicationUserId).SumAsync(c => c.Count);
            HttpContext.Session.SetInt32(StaticDetail.ShoppingCartCount, totalCount);
            
            return RedirectToAction(nameof(Index));
        }

        //public IActionResult Privacy()
        //{
        //    return View();
        //}

        //[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        //public IActionResult Error()
        //{
        //    return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        //}
    }
}
