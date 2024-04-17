using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using SchoolSuppliesStore.Models;
using SchoolSuppliesStore.Repositories;

namespace SchoolSuppliesStore.Controllers
{
    public class ProductImagesController : Controller
    {
        private readonly IProductImagesRepository _productImagesRepository;
        private readonly IProductRepository _productRepository;
        public ProductImagesController(IProductImagesRepository productImagesRepository, IProductRepository productRepository)
        {
            _productImagesRepository = productImagesRepository;
            _productRepository = productRepository;
        }

        public async Task<IActionResult> Index()
        {
            var productImages = await _productImagesRepository.GetImagesAsync();
            return View(productImages);
        }

        public async Task<IActionResult> Add()
        {
            var products = await _productRepository.GetAllAsync();
            ViewBag.Products = new SelectList(products, "ProductId", "Name");
            return View();
        }

        [HttpPost]
        public async Task<IActionResult>Add(ProductImage item)
        {
            if (ModelState.IsValid)
            {
                await _productImagesRepository.AddAsync(item);
                return RedirectToAction("Index");
            }
            var products = await _productRepository.GetAllAsync();
            ViewBag.Products = new SelectList(products, "ProductId", "Name");
            return View(item);
        }
    }
}
