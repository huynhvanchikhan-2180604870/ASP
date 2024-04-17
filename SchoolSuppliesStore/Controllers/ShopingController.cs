using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.IdentityModel.Tokens;
using SchoolSuppliesStore.Data;
using SchoolSuppliesStore.HelperClass;
using SchoolSuppliesStore.Models;
using SchoolSuppliesStore.Repositories;
using SchoolSuppliesStore.Service;
using SchoolSuppliesStore.ShopingModels;
using System.Security.Claims;

namespace SchoolSuppliesStore.Controllers
{
    public class ShopingController : Controller
    {
        private readonly IVnPayService _vnPayService;
        private readonly PaypalClient _paypalClient;
        private readonly IProductRepository _productRepository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public ShopingController(IProductRepository productRepository, ICategoryRepository categoryRepository, ApplicationDbContext context,
            UserManager<ApplicationUser> userManager, PaypalClient paypalClient, IVnPayService vnPayService)
        {
            _vnPayService = vnPayService;
            _paypalClient = paypalClient;
            _productRepository = productRepository;
            _categoryRepository = categoryRepository;
            _context = context;
            _userManager = userManager;
        }

        public IActionResult Index()
        {
            var cart = HttpContext.Session.GetObjectFromJson<ShopingCart>("Cart") ?? new ShopingCart();
            if (cart.Items.Count == 0)
            {
                return View("EmptyCart");
            }
            return View(cart);
        }

        public async Task<IActionResult> AddToCart(int id, int quantity)
        {
            var product = await _productRepository.GetByIdAsync(id);


            if (product == null)
            {
                return NotFound();
            }

            var cartItem = new CartItem
            {
                ProductId = product.ProductId,
                Quantity = quantity,
                Product = product,
                
            };
            var cart = HttpContext.Session.GetObjectFromJson<ShopingCart>("Cart") ?? new ShopingCart();
            cart.AddItem(cartItem);
            HttpContext.Session.SetObjectAsJson("Cart", cart);
            return Redirect(Request.Headers["Referer"].ToString());

        }

        public async Task<IActionResult> RemoveItem(int id)
        {
            var product = await _productRepository.GetByIdAsync(id);


            if (product == null)
            {
                return NotFound();
            }

            var cart = HttpContext.Session.GetObjectFromJson<ShopingCart>("Cart") ?? new ShopingCart();
            cart.RemoveItem(product.ProductId);
            HttpContext.Session.SetObjectAsJson("Cart", cart);
            return Redirect(Request.Headers["Referer"].ToString());
        }
        [Authorize]
        public async Task<IActionResult> Checkout()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToAction("ModalPopup", "Shoping", new { area = "" });
            }

            ViewBag.PaypalClientId = _paypalClient.ClientId;
            return View(new Order());
        }
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Checkout(Order order, string payment = "COD") 
        {
            var user = await _userManager.GetUserAsync(User);
            var cart = HttpContext.Session.GetObjectFromJson<ShopingCart>("Cart");
            if (cart == null)
            {
                return RedirectToAction("Index");
            }
            if(payment == "VNPAY")
            {
                var vnPayModel = new VnPaymentRequest()
                {
                    Amount =(double) cart.Items.Sum(p => p.Price),
                    CreatedDate = DateTime.Now,
                    Description = $"{user.FullName}{user.PhoneNumber}",
                    FullName = user.FullName,
                    OrderId = new Random().Next(1000, 10000)
                };
                return Redirect(_vnPayService.CreatePaymentUrl(HttpContext, vnPayModel));
            }
           
            order.UserId = user.Id;
            order.OrderDate = DateTime.UtcNow;
            order.Status = false;
            order.PaymentType = PaymentType.COD;
            order.Status = false;
            order.OrderDetails = cart.Items.Select(i => new OrderDetail
            {
                ProductId = i.ProductId,
                Quantity = i.Quantity,
                UnitPrice = i.Price,
            }).ToList();

            _context.Orders.Add(order);
            await _context.SaveChangesAsync();
            HttpContext.Session.Remove("Cart");
            return View("OrderCompleted");
        }

        public async Task<IActionResult> ModalPopup()
        {

            var curentUser = await _userManager.GetUserAsync(User);

            return View(curentUser);
        }

        #region paypal
        [HttpPost("/Shoping/create-paypal-order")]
        public async Task<IActionResult> CreatePaypalOrder(CancellationToken cancellationToken)
        {
            var cart = HttpContext.Session.GetObjectFromJson<ShopingCart>("Cart") ?? new ShopingCart();
            var Total = cart.Items.Sum(o => o.Price).ToString();
            var currency = "USD";
            var reference = "DH" + DateTime.Now.Ticks.ToString();
            try
            {
                var response = await _paypalClient.CreateOrder(Total, currency, reference);
                
                return Ok(response);
            }
            catch (Exception ex)
            {
                var error = new { ex.GetBaseException().Message };
                return BadRequest(error);
            }
        }
        [HttpPost("/Shoping/capture-paypal-order")]
        public async Task<IActionResult> CapturePaypalOrder(string orderId, CancellationToken cancellationToken)
        {
            var cart = HttpContext.Session.GetObjectFromJson<ShopingCart>("Cart") ?? new ShopingCart();
            try
            {
                var response = await _paypalClient.CaptureOrder(orderId);
                var order = new Order
                {
                    UserId = _userManager.GetUserId(User),
                    OrderDate = DateTime.Now,
                    PaymentType = PaymentType.PAYPAL,
                    Status = false
                };
                order.OrderDetails = cart.Items.Select(i => new OrderDetail
                {
                    ProductId = i.ProductId,
                    Quantity = i.Quantity,
                    UnitPrice = i.Price,
                }).ToList();
                _context.Orders.Add(order);
                await _context.SaveChangesAsync();
                HttpContext.Session.Remove("Cart");
                return Ok(response);
            }
            catch (Exception ex)
            {
                var error = new { ex.GetBaseException().Message };
                return BadRequest(error);
            }
        }
        #endregion
        public IActionResult OrderCompleted()
        {
            return View();
        }
        [Authorize]
        public IActionResult PaymentFail()
        {
            return View();
        }
        [Authorize]
        public async Task<IActionResult> PaymentCallBack()
        {
            var response = _vnPayService.PaymentExecute(Request.Query);
            var cart = HttpContext.Session.GetObjectFromJson<ShopingCart>("Cart") ?? new ShopingCart();
            if (response == null || response.VnPayResponseCode != "00")
            {
                TempData["Message"] = "Lỗi thanh toán VNPAY:{ response.VnPayResponseCode}";
                return RedirectToAction("PaymentFail");
            }

            var user = await _userManager.GetUserAsync(User);
            try
            {
                var order = new Order()
                {
                    UserId = user.Id,
                    OrderDate = DateTime.Now,
                    PaymentType = PaymentType.VNPAY,
                    Status = false
                };
                order.OrderDetails = cart.Items.Select(i => new OrderDetail
                {
                    ProductId = i.ProductId,
                    Quantity = i.Quantity,
                    UnitPrice = i.Price,
                }).ToList();

                _context.Orders.Add(order);
                await _context.SaveChangesAsync();
                HttpContext.Session.Remove("Cart");
                
            }
            catch (Exception ex)
            {

            }
            TempData["Message"] = $"Thanh toán VNPAY thành công";
            return RedirectToAction("OrderCompleted");
        }
    }
}
