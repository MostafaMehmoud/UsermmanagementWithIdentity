using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UsermmanagementWithIdentity.Models;

namespace UsermmanagementWithIdentity.Controllers
{
    [Authorize(Roles ="Admin")]
    public class RolesController : Controller
    {
        private readonly RoleManager<IdentityRole> _roleManager;
        public RolesController(RoleManager<IdentityRole> roleManager)
        {
            _roleManager= roleManager;
        }
        public async Task<IActionResult> Index()
        {
            var roles = await _roleManager.Roles.ToListAsync();
            return View(roles);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddRole(ViewModelRole model)
        {
            if (!ModelState.IsValid)
            {
                return View(nameof(Index), await _roleManager.Roles.ToListAsync());
            }
            if (await _roleManager.RoleExistsAsync(model.Name))
            {
                ModelState.AddModelError("Name", "Role Is already Exist");
                return View(nameof(Index), await _roleManager.Roles.ToListAsync());
            }
            await _roleManager.CreateAsync(new IdentityRole { Name = model.Name.Trim() });
            return View(nameof(Index), await _roleManager.Roles.ToListAsync());
        }
    }
}
