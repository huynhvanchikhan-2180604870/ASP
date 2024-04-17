using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SchoolSuppliesStore.Models;
using SchoolSuppliesStore.Repositories;
using SchoolSuppliesStore.ShopingModels;
using System.Diagnostics;
using System.Security.Claims;

namespace SchoolSuppliesStore.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IProductRepository _productRepository;
        private readonly UserManager<ApplicationUser> _userManager;
        public HomeController(ILogger<HomeController> logger, IProductRepository productRepository, UserManager<ApplicationUser> userManager)
        {
            _logger = logger;
            _productRepository = productRepository;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {

            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public async Task<IActionResult> Detail(int id)
        {
            var product = await _productRepository.GetByIdAsync(id);

            if (product == null)
            {
                return NotFound();
            }
            CartItem item = new()
            {
                ProductId = product.ProductId,
                Product = product,
            };
            return View(item);
        }

        public async Task<IActionResult> Shop()
        {
            var products = await _productRepository.GetAllAsync();
            return View(products);
        }

        public async Task<IActionResult> ModalPopupAsync()
        {
           
            var curentUser = await _userManager.GetUserAsync(User);

            return View(curentUser);
        }
    }
}
