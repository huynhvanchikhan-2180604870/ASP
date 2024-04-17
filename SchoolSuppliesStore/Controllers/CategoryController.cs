using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SchoolSuppliesStore.Models;
using SchoolSuppliesStore.Repositories;

namespace SchoolSuppliesStore.Controllers
{
    [Authorize(Roles = SD.Role_Admin)]
    public class CategoryController : Controller
    {
        private readonly ICategoryRepository _categoryRepository;
        private readonly UserManager<ApplicationUser> _userManager;
        public CategoryController(ICategoryRepository categoryRepository, UserManager<ApplicationUser> userManager)
        {
            _categoryRepository = categoryRepository;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {

            var categories = await _categoryRepository.GetAllAsync();
            return View(categories);
        }

        public async Task<IActionResult> Add()
        {
            return View();
        }

        [HttpPost]

        public async Task<IActionResult> Add(Category category)
        {
            var cate = await _categoryRepository.GetByIdAsync(category.CategoryId);
            if(cate != null)
            {
                return RedirectToAction("Update", new { id = cate.CategoryId });
            }

            if(ModelState.IsValid)
            {
                await _categoryRepository.AddAsync(category);   
            }
            return RedirectToAction("Index");
        }


        [Authorize(Roles = SD.Role_Admin)]
        public async Task<IActionResult> Delete(int id)
        {
            var cate = await _categoryRepository.GetByIdAsync(id);
            if (cate == null)
            {
                return NotFound();
            }
            return View(cate);
        }
        [Authorize(Roles = SD.Role_Admin)]
        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _categoryRepository.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }
    }
}
