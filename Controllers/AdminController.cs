using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using System.Linq;
using System.Threading.Tasks;
using Project_ASP_NET.Areas.Identity.Data;
using Project_ASP_NET.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace Project_ASP_NET.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly UserManager<SUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public AdminController(UserManager<SUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task<IActionResult> Gebruikers()
        {
            var usersWithRoles = new List<UserViewModel>();

            foreach (var user in await _userManager.Users.ToListAsync())
            {
                var roles = await _userManager.GetRolesAsync(user);
                usersWithRoles.Add(new UserViewModel
                {
                    UserId = user.Id,
                    Username = user.UserName,
                    Roles = roles.ToList()
                });
            }

            return View(usersWithRoles);
        }

        public async Task<IActionResult> AdminCreate()
        {
            var usersWithRoles = new List<UserViewModel>();

            foreach (var user in await _userManager.Users.ToListAsync())
            {
                var roles = await _userManager.GetRolesAsync(user);
                usersWithRoles.Add(new UserViewModel
                {
                    UserId = user.Id,
                    Username = user.UserName,
                    Roles = roles.ToList()
                });
            }

            return View(usersWithRoles);
        }

        [HttpPost]
        public async Task<IActionResult> AssignUserRole(string userId)
        {
            if (string.IsNullOrEmpty(userId))
            {
                return BadRequest("User ID cannot be null or empty.");
            }

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound("User not found.");
            }

            // Check if the "User" role exists
            var roleExists = await _roleManager.RoleExistsAsync("User");
            if (!roleExists)
            {
                return BadRequest("User role does not exist.");
            }

            // Assign "User" role to the user
            var addToRoleResult = await _userManager.AddToRoleAsync(user, "User");
            if (addToRoleResult.Succeeded)
            {
                return RedirectToAction("Gebruikers");
            }
            else
            {
                return BadRequest($"Failed to assign role 'User': {addToRoleResult.Errors}");
            }
        }

        public async Task<IActionResult> AssignAdminRole(string userId)
        {
            if (string.IsNullOrEmpty(userId))
            {
                return BadRequest("User ID cannot be null or empty.");
            }

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound("User not found.");
            }

            // Check if the "Admin" role exists
            var roleExists = await _roleManager.RoleExistsAsync("Admin");
            if (!roleExists)
            {
                return BadRequest("Admin role does not exist.");
            }

            // Assign "Admin" role to the user
            var addToRoleResult = await _userManager.AddToRoleAsync(user, "Admin");
            if (addToRoleResult.Succeeded)
            {
                return RedirectToAction("Gebruikers");
            }
            else
            {
                return BadRequest($"Failed to assign role 'Admin': {addToRoleResult.Errors}");
            }
        }

        public async Task<IActionResult> RemoveAdminRole(string userId)
        {
            if (string.IsNullOrEmpty(userId))
            {
                return BadRequest("User ID cannot be null or empty.");
            }

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound("User not found.");
            }

            // Remove "Admin" role from the user
            var removeFromRoleResult = await _userManager.RemoveFromRoleAsync(user, "Admin");
            if (removeFromRoleResult.Succeeded)
            {
                return RedirectToAction("Gebruikers");
            }
            else
            {
                return BadRequest($"Failed to remove role 'Admin': {removeFromRoleResult.Errors}");
            }
        }
    }
}