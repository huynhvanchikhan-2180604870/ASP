using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SchoolSuppliesStore.Data;
using SchoolSuppliesStore.Models;

namespace SchoolSuppliesStore.Areas.Admin.Controllers
{
    [Area("admin")]
    [Authorize(Roles = SD.Role_Admin)]
    public class OrderController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public OrderController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }
        public async Task<IActionResult> Index()
        {
            var orders = await _context.Orders.Include(p => p.User).ToListAsync();
            return View(orders);
        }

        public async Task<IActionResult> ConfirmOrder(int id)
        {
            var order = await _context.Orders.Include(u => u.User).Include(o => o.OrderDetails).FirstOrDefaultAsync(x => x.OrderId == id);
            if (order == null)
            {
                return NotFound();
            }

            foreach (var product in order.OrderDetails)
            {
                var item = await _context.Products.FirstOrDefaultAsync(x => x.ProductId == product.ProductId);
                if (item != null)
                {
                    product.Product = item;
                }
            }

            return View(order);
        }

        [HttpPost]
        public async Task<IActionResult> ConfirmOrder(Order order)
        {
            if (ModelState.IsValid)
            {
                order.Status = true;
                _context.Orders.Update(order);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction("Index");
        }
    }
}
