using Eateroo.Data;
using Eateroo.Models;
using Eateroo.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace Eateroo.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class SubCategoryController : Controller
    {
        private readonly ApplicationDbContext _db;

        public SubCategoryController(ApplicationDbContext db)
        {
            _db = db;
        }

        // INDEX --------------------------------------------------------------
        // GET
        public async Task<IActionResult> Index()
        {
            return View(await _db.SubCategory.Include(s => s.Category).ToListAsync());
        }

        // CREATE -------------------------------------------------------------
        // GET
        public async Task<IActionResult> Create()
        {
            var viewModel = await CreateSubCategoryAndCategoryViewModelInstance();
            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(SubCategoryAndCategoryViewModel viewModel)
        {
            if (viewModel == null)
            {
                return UnprocessableEntity();
            }

            if (!ModelState.IsValid)
            {
                var newViewModel = await CreateSubCategoryAndCategoryViewModelInstance(viewModel.SubCategory);
                return View(newViewModel);
            }

            var isSubCategoryExists = _db.SubCategory
                .Include(s => s.Category)
                .Any(s => s.Name == viewModel.SubCategory.Name && s.CategoryId == viewModel.SubCategory.CategoryId);

            if (isSubCategoryExists)
            {
                var newViewModel = await CreateSubCategoryAndCategoryViewModelInstance(viewModel.SubCategory);
                newViewModel.Message = "Error : The Sub Category Name already exists for the selected Category. Please enter another name.";
                return View(newViewModel);
            }

            _db.SubCategory.Add(viewModel.SubCategory);
            await _db.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        [ActionName("GetSubCategories")]
        public async Task<IActionResult> GetSubCategories(int id)
        {
            var subCategories = await (from subCategory in _db.SubCategory
                                       where subCategory.CategoryId == id
                                       orderby subCategory.Name
                                       select subCategory).ToListAsync();

            return Json(new SelectList(subCategories, "Id", "Name"));
        }

        // EDIT ---------------------------------------------------------------
        // GET
        public async Task<IActionResult> Edit(int id)
        {
            var subCategory = await _db.SubCategory.FindAsync(id);
            if (subCategory == null)
            {
                return NotFound();
            }

            var viewModel = await CreateSubCategoryAndCategoryViewModelInstance(subCategory);
            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(SubCategoryAndCategoryViewModel viewModel)
        {
            if (viewModel == null)
            {
                return UnprocessableEntity();
            }

            if (!ModelState.IsValid)
            {
                var newViewModel = await CreateSubCategoryAndCategoryViewModelInstance(viewModel.SubCategory);
                return View(newViewModel);
            }

            var isSubCategoryExists = _db.SubCategory
                .Include(s => s.Category)
                .Any(s => s.Name == viewModel.SubCategory.Name && s.CategoryId == viewModel.SubCategory.CategoryId);

            if (isSubCategoryExists)
            {
                var newViewModel = await CreateSubCategoryAndCategoryViewModelInstance(viewModel.SubCategory);
                newViewModel.Message = "Error : The Sub Category Name already exists for the selected Category. Please enter another name.";
                return View(newViewModel);
            }

            var subCategory = await _db.SubCategory.FindAsync(viewModel.SubCategory.Id);
            if (subCategory == null)
            {
                return NotFound();
            }

            // Update only the specific properties
            subCategory.CategoryId = viewModel.SubCategory.CategoryId;
            subCategory.Name = viewModel.SubCategory.Name;
            await _db.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        // DETAIL ---------------------------------------------------------------
        // GET
        public async Task<IActionResult> Detail(int id)
        {
            var subCategory = await _db.SubCategory.Include(s => s.Category).FirstOrDefaultAsync(s => s.Id == id);
            if (subCategory == null)
            {
                return NotFound();
            }
            
            return View(subCategory);
        }

        // DELETE -------------------------------------------------------------
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var subCategory = await _db.SubCategory.FindAsync(id);
            if (subCategory == null)
            {
                return NotFound();
            }

            _db.SubCategory.Remove(subCategory);
            await _db.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }


        private async Task<SubCategoryAndCategoryViewModel> CreateSubCategoryAndCategoryViewModelInstance(SubCategory subCategory = null)
        {
            return new SubCategoryAndCategoryViewModel()
            {
                CategoryList = await _db.Category.ToListAsync(),
                SubCategory = subCategory ?? new SubCategory(),
                SubCategoryList = await _db.SubCategory.OrderBy(s => s.Name).Select(s => s.Name).ToListAsync()
            };
        }
    }
}