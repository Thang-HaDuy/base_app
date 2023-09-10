using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using App.Data;
using App.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace App.Controllers
{
    [Route("/DbManage/[Action]")]
    public class DbManage : Controller
    {
        
        private readonly ILogger<DbManage> _logger;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<AppUser> _userManager;
        private readonly DataDbContext _dbContext;

        public DbManage(ILogger<DbManage> logger, RoleManager<IdentityRole> roleManager, DataDbContext dbContext, UserManager<AppUser> userManager)
        {
            _logger = logger;
            _roleManager = roleManager;
            _userManager = userManager;
            _dbContext = dbContext;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [Authorize(Roles = RoleName.Administrator)]
        public async Task<IActionResult> Migrate()
        {
            await _dbContext.Database.MigrateAsync();
            return RedirectToAction(nameof(Index));
        }

            
        public async Task<IActionResult> Seedata()
        {
            var roleNames = typeof(RoleName).GetFields().ToList();
            foreach (var role in roleNames)
            {
                var rolename = (string)role.GetRawConstantValue();
                var rfound = await _roleManager.FindByNameAsync(rolename);
                if (rfound == null)
                {
                    await _roleManager.CreateAsync(new IdentityRole(rolename));
                }
            }

            //admin, pass=admin@123, admin@gmail.com
            var useradmin = await _userManager.FindByNameAsync("admin");
            if (useradmin == null)
            {
                useradmin = new AppUser()
                {
                    UserName = "admin",
                    Email = "admin@gmail.com",
                    EmailConfirmed = true,   
                };

                await _userManager.CreateAsync(useradmin, "Admin@123");
                await _userManager.AddToRoleAsync(useradmin, RoleName.Administrator);
            
                return RedirectToAction("Index");
            }
            else
            {
              var user = await _userManager.GetUserAsync(this.User);
              if (user == null) return this.Forbid();
              var role = await _userManager.GetRolesAsync(user);

              if (!role.Any(r =>r == RoleName.Administrator))
              {
                return this.Forbid();
              }
            }
            return this.Forbid();
        }   


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View("Error!");
        }
    }
}