using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SchoolSuppliesStore.Areas.Admin.Models;
using SchoolSuppliesStore.Data;
using SchoolSuppliesStore.Models;

namespace SchoolSuppliesStore.Areas.Admin.Controllers
{
    [Area("admin")]
    [Authorize(Roles =SD.Role_Admin)]
    public class AccountController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public AccountController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
        }
        public async Task<IActionResult> Index()
        {
            var users = await _context.Users
                            .ToListAsync();
            // Load roles for each user separately
            return View(users);
        }

        public async Task<IActionResult> UpdateAccount(string id)
        {

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == id);
            if (user == null)
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
            var roleAll = await _context.Roles.ToListAsync();
            
            return View(userAndRoles);
        }
    }
}
