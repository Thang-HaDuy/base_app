using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using App.Models;
using App.Data;
using Microsoft.AspNetCore.Identity;

namespace App.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly UserManager<AppUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;

    public HomeController(ILogger<HomeController> logger, UserManager<AppUser> userManager, RoleManager<IdentityRole> roleManager)
    {
        _logger = logger;
        _roleManager = _roleManager;
        _userManager = userManager;
    }

    public IActionResult Index()
    {
        return View();
    }

    public async Task<IActionResult> SeedataAsync()
    {
        var roleName = typeof(RoleName).GetFields().ToList();
        foreach (var role in roleName)
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
        }
        return RedirectToAction("Index", "Home");
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
