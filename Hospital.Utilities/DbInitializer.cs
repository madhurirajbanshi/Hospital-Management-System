using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hospital.Models;
using Hospital.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Hospital.Utilities;
public class DbInitializer : IDbInitializer
{
    private readonly UserManager<IdentityUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly ApplicationDbContext _Context;

    public DbInitializer(UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager, ApplicationDbContext context)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _Context = context;
    }

    public void Initialize()
    {
#pragma warning disable S2737 
        try
        {
            if (_Context.Database.GetPendingMigrations().Any())
            {
                _Context.Database.Migrate();
            }
        }
        catch (Exception)
        {
            throw;
        }
#pragma warning restore S2737 

        if (!_roleManager.RoleExistsAsync(WebsiteRoles.WebSiteAdmin).GetAwaiter().GetResult())
        {
            _roleManager.CreateAsync(new IdentityRole(WebsiteRoles.WebSiteAdmin)).GetAwaiter().GetResult();
            _roleManager.CreateAsync(new IdentityRole(WebsiteRoles.WebSitePatient)).GetAwaiter().GetResult();
            _roleManager.CreateAsync(new IdentityRole(WebsiteRoles.WebSiteDoctor)).GetAwaiter().GetResult();
            _userManager.CreateAsync(new ApplicationUser
            {
                UserName = "Madhuri",
                Email = "madhuri@gmail.com"

            }, "madhuri123").GetAwaiter().GetResult();
            ApplicationUser? Appuser = _Context.ApplicationUsers.FirstOrDefault(x => x.Email == "madhuri@gmail.com");
            if (Appuser != null)
            {
                _userManager.AddToRoleAsync(Appuser, WebsiteRoles.WebSiteAdmin);

            }
        }
    }
}
