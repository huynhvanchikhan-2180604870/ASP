using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using SchoolSuppliesStore.Models;
using SchoolSuppliesStore.Repositories;
using SchoolSuppliesStore.ShopingModels;
using System.Drawing.Printing;
using System.Security.Claims;
using static System.Net.Mime.MediaTypeNames;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace SchoolSuppliesStore.Controllers
{

    public class ProductController : Controller
    {
        private readonly IProductRepository _productRepository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly IProductImagesRepository _productImagesRepository;
        private readonly UserManager<ApplicationUser> _userManager;
        public ProductController(IProductRepository productRepository, ICategoryRepository categoryRepository, IProductImagesRepository productImagesRepository,
            UserManager<ApplicationUser> userManager)
        {
            _productRepository = productRepository;
            _categoryRepository = categoryRepository;
            _productImagesRepository = productImagesRepository;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            if (User.Identity.IsAuthenticated)
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier); // Get user ID
                if (userId != null)
                {
                    var user = await _userManager.FindByIdAsync(userId); // Find user by ID
                    if (user != null && await _userManager.IsInRoleAsync(user, SD.Role_Customer))
                    {
                        return RedirectToAction("Shop");
                    }
                }
            }
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Shop");
            }
            var products = await _productRepository.GetAllAsync();

            return View(products);
        }
        [Authorize(Roles = SD.Role_Admin)]
        public async Task<IActionResult> Add()
        {
            var categories = await _categoryRepository.GetAllAsync();
            ViewBag.Categories = new SelectList(categories, "CategoryId", "Name");
            return View();
        }
        [Authorize(Roles = SD.Role_Admin)]
        [HttpPost]

        public async Task<IActionResult> Add(Product product, IFormFile Cover)
        {
            if (ModelState.IsValid)
            {
                if (Cover != null)
                {
                    // Lưu hình ảnh đại diện tham khảo bài 02 hàm SaveImage
                    product.Cover = await SaveImage(Cover);
                }
                await _productRepository.AddAsync(product);
                return RedirectToAction(nameof(Index));
            }
            // Nếu ModelState không hợp lệ, hiển thị form với dữ liệu đã nhập
            var categories = await _categoryRepository.GetAllAsync();
            ViewBag.Categories = new SelectList(categories, "CategoryId", "Name");
            return View(product);
        }

        // Viết thêm hàm SaveImage (tham khảo bài 02)
        private async Task<string> SaveImage(IFormFile image)
        {
            var savePath = Path.Combine("wwwroot/images", image.FileName); //Thay đổi đường dẫn theo cấu hình của bạn

            using (var fileStream = new FileStream(savePath, FileMode.Create))
            {
                await image.CopyToAsync(fileStream);
            }
            return "/images/" + image.FileName; // Trả về đường dẫn tương đối
        }

        public async Task<IActionResult> Detail(int id)
        {
            var product = await _productRepository.GetByIdAsync(id);
            if (product == null)
            {
                return RedirectToAction("Add");
            }
            return View(product);
        }
        [Authorize(Roles = SD.Role_Admin)]
        public async Task<IActionResult> Update(int id)
        {
            var product = await _productRepository.GetByIdAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            var categories = await _categoryRepository.GetAllAsync();
            ViewBag.Categories = new SelectList(categories, "CategoryId", "Name", product.CategoryId);

            return View(product);
        }
        [Authorize(Roles = SD.Role_Admin)]
        [HttpPost]
        public async Task<IActionResult> Update(int id, Product product, IFormFile Cover)
        {
            ModelState.Remove("Cover");
            if (id != product.ProductId)
            {
                return NotFound();
            }
            if (ModelState.IsValid)
            {
                var existingProduct = await _productRepository.GetByIdAsync(id);
                if (Cover == null)
                {
                    product.Cover = existingProduct.Cover;
                }
                else
                {
                    // Lưu hình ảnh mới
                    product.Cover = await SaveImage(Cover);
                }
                existingProduct.Name = product.Name;
                existingProduct.Price = product.Price;
                existingProduct.Description = product.Description;
                existingProduct.CategoryId = product.CategoryId;
                existingProduct.Cover = product.Cover;
                await _productRepository.UpdateAsync(existingProduct);
                return RedirectToAction(nameof(Index));
            }
            var categories = await _categoryRepository.GetAllAsync();
            ViewBag.Categories = new SelectList(categories, "CategotyId", "Name");
            return View(product);
        }
        [Authorize(Roles = SD.Role_Admin)]
        public async Task<IActionResult> Delete(int id)
        {
            var product = await _productRepository.GetByIdAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            return View(product);
        }
        [Authorize(Roles = SD.Role_Admin)]
        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _productRepository.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }


        public async Task<IActionResult> Shop(int? category, int? product, int page = 1)
        {
            const int pageSize = 9;
            var products = await _productRepository.GetAllAsync();
            var categories = await _categoryRepository.GetAllAsync();
            var paginatedProducts = products.Skip((page - 1) * pageSize).Take(pageSize).ToList();
            
            var viewModel = new ViewModels
            {
                Products = paginatedProducts,
                Categories = categories,
                // Add other necessary properties to the viewModel
                PageNumber = page,
                PageSize = pageSize,
                TotalProductsCount = products.Count() // This can be optimized
            };

            if (category.HasValue)
            {
                var categoryId = category.Value;
                var filteredProducts = products.Where(x => x.CategoryId == categoryId).ToList();
                viewModel.Products = filteredProducts;
            }
            ViewBag.Sorting = new SelectList(new[]
            {new SelectListItem { Text = "Nothing", Value = "sorttype1" },
                new SelectListItem { Text = "Prices from low to high", Value = "sorttype1" },
                new SelectListItem { Text = "Prices from high to low", Value = "sorttype2" },
                new SelectListItem { Text = "Names alphabetically", Value = "sorttype3" }
            }, "Value", "Text"); // Pass the selected sorting type
            
            
            return View(viewModel);
        }

        public async Task<IActionResult> ProductDetail(int id)
        {
            var product = await _productRepository.GetByIdAsync(id);

            // Check if product exists
            if (product == null)
            {
                return NotFound();
            }

            // Create a new ShoppingCartItem
            var shoppingCartItem = new CartItem
            {
                ProductId = product.ProductId,
                Product = product,

            };

            return View(shoppingCartItem);
        }

        public async Task<IActionResult> Search(string? value, int page = 1)
        {
            const int pageSize = 9;
            var products = await _productRepository.GetAllAsync();
            var categories = await _categoryRepository.GetAllAsync();
            var paginatedProducts = products.Skip((page - 1) * pageSize).Take(pageSize).ToList();

            var viewModel = new ViewModels
            {
                Products = paginatedProducts,
                Categories = categories,
                // Add other necessary properties to the viewModel
                PageNumber = page,
                PageSize = pageSize,
                TotalProductsCount = products.Count() // This can be optimized
            };
            ViewBag.Sorting = new SelectList(new[]
            {new SelectListItem { Text = "Nothing", Value = "sorttype1" },
                new SelectListItem { Text = "Prices from low to high", Value = "sorttype1" },
                new SelectListItem { Text = "Prices from high to low", Value = "sorttype2" },
                new SelectListItem { Text = "Names alphabetically", Value = "sorttype3" }
            }, "Value", "Text"); // Pass the selected sorting type

            if (value != null)
            {
                var searchTerm = value.Trim().ToUpper(); // Loại bỏ khoảng trắng thừa và chuyển đổi giá trị nhập vào thành chuỗi tìm kiếm
                var filteredProducts = products.Where(x => x.Name.ToUpper().Contains(searchTerm)).ToList();
                viewModel.Products = filteredProducts;
            }


            return View(viewModel);
        }

        public async Task<IActionResult> Sorting(string? sortingtype, int page = 1)
        {
            const int pageSize = 9;
            var products = await _productRepository.GetAllAsync();
            var categories = await _categoryRepository.GetAllAsync();
            var paginatedProducts = products.Skip((page - 1) * pageSize).Take(pageSize).ToList();

            var viewModel = new ViewModels
            {
                Products = paginatedProducts,
                Categories = categories,
                // Add other necessary properties to the viewModel
                PageNumber = page,
                PageSize = pageSize,
                TotalProductsCount = products.Count() // This can be optimized
            };

            // Populate the dropdown list with sorting options
            ViewBag.Sorting = new SelectList(new[]
            { new SelectListItem { Text = "Nothing", Value = "sorttype1" },
                new SelectListItem { Text = "Prices from low to high", Value = "sorttype1" },
                new SelectListItem { Text = "Prices from high to low", Value = "sorttype2" },
                new SelectListItem { Text = "Names alphabetically", Value = "sorttype3" }
            }, "Value", "Text", sortingtype); // Pass the selected sorting type


            // Apply sorting based on the selected option
            if (!string.IsNullOrEmpty(sortingtype))
            {
                switch (sortingtype)
                {
                    case "sorttype": // No sorting
                        break;
                    case "sorttype1": // Prices from low to high
                        viewModel.Products = viewModel.Products.OrderBy(p => p.Price).ToList();
                        break;
                    case "sorttype2": // Prices from high to low
                        viewModel.Products = viewModel.Products.OrderByDescending(p => p.Price).ToList();
                        break;
                    case "sorttype3": // Names alphabetically
                        viewModel.Products = viewModel.Products.OrderBy(p => p.Name).ToList();
                        break;
                    default:
                        break;
                }
            }
             return View(viewModel);
        }

        
    }
}
