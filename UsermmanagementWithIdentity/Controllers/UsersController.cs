using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
using System.Net.Mail;
using UsermmanagementWithIdentity.Models;

namespace UsermmanagementWithIdentity.Controllers
{
    [Authorize(Roles ="Admin")]
    public class UsersController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        public UsersController(UserManager<ApplicationUser> userManager, 
            RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }
        public async Task<IActionResult> Index()
        {
            // Fetch all users first
            var users = await _userManager.Users.ToListAsync();

            // Map users to the UserViewModel with roles
            var userViewModels = new List<UserViewModel>();
            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);
                userViewModels.Add(new UserViewModel
                {
                    Id = user.Id,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    UserName = user.UserName,
                    Email = user.Email,
                    Roles = roles
                });
            }

            return View(userViewModels);
        }
        [HttpGet]
        public async Task<IActionResult> Edit(string userId)
        {
            var user=await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound();
            }
            var model = new ProfileUserFromViewModel
            {
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                UserName = user.UserName,
                Id = user.Id
            };
            return View(model);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(ProfileUserFromViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var userWithSameEmail=await _userManager.FindByEmailAsync(model.Email);
            if (userWithSameEmail != null && userWithSameEmail.Id != model.Id)
            {
                ModelState.AddModelError("Email", "The email is assign to Anothar user");
                return View(model);
            }
            var userWithSameUserName = await _userManager.FindByNameAsync(model.UserName);
            if (userWithSameUserName != null && userWithSameUserName.Id != model.Id)
            {
                ModelState.AddModelError("UserName", "The username is assign to Anothar user");
                return View(model);
            }
            var user=await _userManager.FindByIdAsync(model.Id);    
            user.FirstName= model.FirstName;    
            user.LastName= model.LastName;
            user.UserName= model.UserName;
            user.Email= model.Email;
            await _userManager.UpdateAsync(user);
            return RedirectToAction(nameof(Index));
        }
        [HttpGet]
        public async Task<IActionResult> AddUser()
        {
            var roles=await _roleManager.Roles.Select(r=>new RoleViewModel
            {
                RoleId = r.Id,
                RoleName=r.Name
            }).ToListAsync();
            AddUserViewModel model=new AddUserViewModel()
            {
                Roles = roles
            };
            return View(model);  
        }
        [HttpPost]  
        public async Task<IActionResult> AddUser(AddUserViewModel model)
        {
            ModelState.Remove("Roles");
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            if (!model.Roles.Any(r => r.IsSelected))
            {
                ModelState.AddModelError("Roles", "Please select at least one role");
                return View(model);
            }
            if(await _userManager.FindByEmailAsync(model.Email) != null)
            {
                ModelState.AddModelError("Email", "The Email Is Already Exist");
                return View(model);
            }
            if(await _userManager.FindByNameAsync(model.UserName) != null)
            {
                ModelState.AddModelError("UserName", "The User Name Is Already Exist");
                return View(model);
            }
            var user = new ApplicationUser();
            user.FirstName = model.FirstName;
            user.LastName = model.LastName;
            user.Email = model.Email;
            user.UserName = new MailAddress(model.Email).User;
            var result = await _userManager.CreateAsync(user, model.Password);
            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("Roles", error.Description);
                }
                return View(model);
            }
            await _userManager.AddToRolesAsync(user, model.Roles.Where(s => s.IsSelected).Select(r => r.RoleName));
            return RedirectToAction(nameof(Index));
        }
        [HttpGet]
        public async Task<IActionResult> ManageUserRole(string userId)
        {
            var user=await _userManager.FindByIdAsync(userId);    
            if (user == null)
            {
                return NotFound();
            }
            var roles=await _roleManager.Roles.ToListAsync();
            var ViewModel = new UserRoleViewModel
            {
                UserId = user.Id,
                UserName = user.UserName,
                Roles = roles.Select(role => new RoleViewModel
                {
                    RoleId = role.Id,
                    RoleName = role.Name,
                    IsSelected = _userManager.IsInRoleAsync(user, role.Name).Result
                }).ToList()
            };
            return View(ViewModel);
        }
        public async Task<IActionResult> ManageUserRole(UserRoleViewModel userRole)
        {
            var user = await _userManager.FindByIdAsync(userRole.UserId);
            if (user == null)
            {
                return NotFound();
            }
            var userRoles = await _userManager.GetRolesAsync(user);
            foreach (var role in userRole.Roles)
            {
                if(userRoles.Any(r=>r==role.RoleName)&&!role.IsSelected)
                {
                    await _userManager.RemoveFromRoleAsync(user,role.RoleName);
                }
                if (!userRoles.Any(r => r == role.RoleName) && role.IsSelected)
                {
                    await _userManager.AddToRoleAsync(user, role.RoleName);
                }
            }
            return RedirectToAction(nameof(Index));
        }
        [HttpPost]
        public async Task<IActionResult> DeleteUser(string userId)
        {
            try
            {
                var user =await _userManager.FindByIdAsync(userId);
                if (user == null)
                {
                    return Json(new { success = false, message = "The user is not found in database" });
                }
                await _userManager.DeleteAsync(user);
                return Json(new { success = true, message ="Done" });
            }
            catch (Exception ex)
            {
                // If something goes wrong
                return Json(new { success = false, message = ex.Message });
            }
        }
    }
}
