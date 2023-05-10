using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using OnlineSellingStore.DataAccess.Data;
using OnlineSellingStore.Models;
using OnlineSellingStore.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineSellingStore.DataAccess.DbInitializer
{
    public class DbInitializer : IDbInitializer
    {
        public readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ApplicationDbContext _db;

        public DbInitializer(
            UserManager<IdentityUser> userManager,
            RoleManager<IdentityRole> roleManager, 
            ApplicationDbContext db)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _db = db;
        }

        public void Initialize()
        {
            //1. Migrationst if they are not applied
            try
            {
                if(_db.Database.GetPendingMigrations().Count() > 0)
                { 
                    _db.Database.Migrate();
                }
            }catch (Exception ex)
            {

                Console.WriteLine("----------------------------------------------------------------------");
                Console.WriteLine(ex.Message);
                Console.WriteLine("----------------------------------------------------------------------");
                Console.WriteLine(ex.Source);
                Console.WriteLine("----------------------------------------------------------------------");

            }


            //2. Create roles if they are not created
            if (!_roleManager.RoleExistsAsync(SD.Role_Customer).GetAwaiter().GetResult())
            {
                _roleManager.CreateAsync(new IdentityRole(SD.Role_Customer)).GetAwaiter().GetResult();
                _roleManager.CreateAsync(new IdentityRole(SD.Role_Employee)).GetAwaiter().GetResult();
                _roleManager.CreateAsync(new IdentityRole(SD.Role_Admin)).GetAwaiter().GetResult();
                _roleManager.CreateAsync(new IdentityRole(SD.Role_Company)).GetAwaiter().GetResult();

                //3. If roles are not created, then we will create admin user as well
                _userManager.CreateAsync(new ApplicationUser
                {
                    UserName = "admin@polclardstore.com",
                    Email = "polclard@polclardstore.com",
                    Name = "Polclard Pol",
                    PhoneNumber = "1234567890",
                    StreetAddress = "Test 222 Avenue",
                    State = "IL",
                    PostalCode = "21411",
                    City = "Chicago",
                    EmailConfirmed = true,
                }, "Admin123*").GetAwaiter().GetResult();

                ApplicationUser user = _db.applicationUsers.FirstOrDefault(u => u.Email == "polclard@polclardstore.com");
                _userManager.AddToRoleAsync(user, SD.Role_Admin).GetAwaiter().GetResult();
            }
            return;
        }
    }
}
