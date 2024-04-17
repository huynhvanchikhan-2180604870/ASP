using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SchoolSuppliesStore.Areas.Admin.Models;
using SchoolSuppliesStore.Data;
using SchoolSuppliesStore.Models;

namespace SchoolSuppliesStore.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin)]
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public HomeController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }
        [Authorize]
        public async Task<IActionResult> Index()
        {
            var orders = await _context.Orders.ToListAsync();
            var orderDatail = await _context.OrderDetails.ToListAsync();
            var totals = orderDatail.Sum(i => i.UnitPrice);
            var users = await _userManager.Users.Select(u => new { Id = u.Id, UserName = u.UserName })
                                     .ToListAsync();
            Dashboard dashboard = new Dashboard()
            {
                orderCount = orders.Count,
                totals = (decimal)totals,
                countUser = users.Count
            };
            return View(dashboard);
        }
        [Authorize]
        public async Task<IActionResult> AccountManager()
        {
            var users = await _context.Users
                            .ToListAsync();
            // Load roles for each user separately
            return View(users);
        }
        [Authorize]
        public async Task<IActionResult> UpdateAccount(string id)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == id);
            if (user == null || !ModelState.IsValid)
            {
                return NotFound();
            }

            var roleIds = await _context.UserRoles
                .Where(ur => ur.UserId == user.Id)
                .Select(ur => ur.RoleId)
                .ToListAsync();

            var roles = await _context.Roles
                .Where(r => roleIds.Contains(r.Id))
                .ToListAsync();

            var userAndRoles = new UsersRole
            {
                UserRoles = roles,
                User = user
            };

            return View(userAndRoles);
        }
        [Authorize]
        public async Task<IActionResult> OrderManager()
        {
            var orders = await _context.Orders.Include(p => p.User).ToListAsync();
            return View(orders);
        }
    }
}
