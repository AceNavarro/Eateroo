using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Eateroo.Data;
using Eateroo.Models;
using Eateroo.Models.ViewModels;
using Eateroo.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Eateroo.Areas.Admin.Controllers
{
    [Authorize(Roles = StaticDetail.ManagerUser)]
    [Area("Admin")]
    [ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
    public class MenuItemController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly IHostingEnvironment _hostingEnvironment;

        [BindProperty]
        public MenuItemViewModel MenuItemVM { get; set; }

        public MenuItemController(ApplicationDbContext db, IHostingEnvironment hostingEnvironmewnt)
        {
            _db = db;
            _hostingEnvironment = hostingEnvironmewnt;
            MenuItemVM = new MenuItemViewModel()
            {
                MenuItem = new MenuItem(),
                Categories = _db.Category.ToList()
            };
        }

        // INDEX --------------------------------------------------------------
        // GET
        public async Task<IActionResult> Index()
        {
            var menuItems = await _db.MenuItem
                .Include(m => m.Category)
                .Include(m => m.SubCategory)
                .ToListAsync();
            return View(menuItems);
        }

        // CREATE -------------------------------------------------------------
        // GET
        public IActionResult Create()
        {
            return View(MenuItemVM);
        }
        
        // POST
        [HttpPost, ActionName("Create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> PostCreate()
        {
            if (!ModelState.IsValid)
            {
                return View(MenuItemVM);
            }

            var selectedSubCategory = Request.Form["selectSubCategoryList"].ToString();
            var subCategoryFromDb = await _db.SubCategory.FirstOrDefaultAsync(s => s.Name == selectedSubCategory);
            MenuItemVM.MenuItem.SubCategoryId = subCategoryFromDb.Id;

            _db.MenuItem.Add(MenuItemVM.MenuItem);
            // Save is required here in order to get the correct Id.
            await _db.SaveChangesAsync();

            string webRootPath = _hostingEnvironment.WebRootPath;
            var files = Request.Form.Files;
            var menuItemId = MenuItemVM.MenuItem.Id;
            
            if (files.Count > 0)
            {
                var imagesPath = Path.Combine(webRootPath, "images");
                var extension = Path.GetExtension(files[0].FileName);

                using (var fileStream = new FileStream(Path.Combine(imagesPath, menuItemId + extension), FileMode.Create))
                {
                    await files[0].CopyToAsync(fileStream);
                }

                MenuItemVM.MenuItem.Image = Path.Combine(@"\images", menuItemId + extension);
            }
            else
            {
                var sourcePath = Path.Combine(webRootPath, "images", StaticDetail.DefaultFoodImage);
                System.IO.File.Copy(sourcePath, Path.Combine(webRootPath, "images", menuItemId + ".png"));
                MenuItemVM.MenuItem.Image = Path.Combine(@"\images", menuItemId + ".png");
            }

            await _db.SaveChangesAsync();
            
            return RedirectToAction(nameof(Index));
        }

        // EDIT -------------------------------------------------------------
        // GET
        public async Task<IActionResult> Edit(int id)
        {
            var menuItem = await _db.MenuItem
                .Include(m => m.Category)
                .Include(m => m.SubCategory)
                .SingleOrDefaultAsync(m => m.Id == id);

            if (menuItem == null)
            {
                return NotFound();
            }

            MenuItemVM.MenuItem = menuItem;
            MenuItemVM.SubCategories = await _db.SubCategory.Where(s => s.CategoryId == menuItem.CategoryId).ToListAsync();

            return View(MenuItemVM);
        }

        // POST
        [HttpPost, ActionName("Edit")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> PostEdit()
        {
            if (!ModelState.IsValid)
            {
                return View(MenuItemVM);
            }

            var menuItemId = MenuItemVM.MenuItem.Id;
            var menuItemFromDb = await _db.MenuItem.FindAsync(menuItemId);
            if (menuItemFromDb == null)
            {
                return NotFound();
            }

            var files = Request.Form.Files;

            // Update the image only if a file has been selected
            if (files.Count > 0)
            {
                string webRootPath = _hostingEnvironment.WebRootPath;
                var existingFilePath = Path.Combine(webRootPath, menuItemFromDb.Image.TrimStart(Path.DirectorySeparatorChar));
                System.IO.File.Delete(existingFilePath);

                var imagesPath = Path.Combine(webRootPath, "images");
                var extension = Path.GetExtension(files[0].FileName);

                using (var fileStream = new FileStream(Path.Combine(imagesPath, menuItemId + extension), FileMode.Create))
                {
                    await files[0].CopyToAsync(fileStream);
                }

                menuItemFromDb.Image = Path.Combine(@"\images", menuItemId + extension);
            }

            var selectedSubCategory = Request.Form["selectSubCategoryList"].ToString();
            var subCategoryFromDb = await _db.SubCategory.FirstOrDefaultAsync(s => s.Name == selectedSubCategory);
            menuItemFromDb.SubCategoryId = subCategoryFromDb.Id;

            menuItemFromDb.Name = MenuItemVM.MenuItem.Name;
            menuItemFromDb.Description = MenuItemVM.MenuItem.Description;
            menuItemFromDb.Price = MenuItemVM.MenuItem.Price;
            menuItemFromDb.CategoryId = MenuItemVM.MenuItem.CategoryId;
            menuItemFromDb.Spiciness = MenuItemVM.MenuItem.Spiciness;

            _db.MenuItem.Update(menuItemFromDb);
            await _db.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        // DETAIL -------------------------------------------------------------
        // GET
        public async Task<IActionResult> Detail(int id)
        {
            var menuItem = await _db.MenuItem
                .Include(m => m.Category)
                .Include(m => m.SubCategory)
                .SingleOrDefaultAsync(m => m.Id == id);

            if (menuItem == null)
            {
                return NotFound();
            }

            MenuItemVM.MenuItem = menuItem;
            MenuItemVM.SubCategories = await _db.SubCategory.Where(s => s.CategoryId == menuItem.CategoryId).ToListAsync();

            return View(MenuItemVM);
        }

        // DELETE -------------------------------------------------------------
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var menuItem = await _db.MenuItem.FindAsync(id);
            if (menuItem == null)
            {
                return NotFound();
            }

            string webRootPath = _hostingEnvironment.WebRootPath;
            var imagePath = Path.Combine(webRootPath, menuItem.Image.TrimStart(Path.DirectorySeparatorChar));
            System.IO.File.Delete(imagePath);

            _db.MenuItem.Remove(menuItem);
            await _db.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
    }
}