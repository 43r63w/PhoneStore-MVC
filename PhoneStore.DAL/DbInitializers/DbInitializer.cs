using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PhoneStore.DAL.Data;
using PhoneStore.Entities;
using PhoneStore.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhoneStore.DAL.DbInitializers
{
    public class DbInitializer : IDbInitializer
    {
        private readonly ApplicationdDBContext _dbContext;
        private readonly UserManager<IdentityUser> _userManager;    
        private readonly RoleManager<IdentityRole> _roleManager;

        public DbInitializer(ApplicationdDBContext dbContext, 
            UserManager<IdentityUser> userManager, 
            RoleManager<IdentityRole> roleManager)
        {
            _dbContext = dbContext;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task Initialize()
        {

            try
            {
                _dbContext.Database.EnsureCreated();



                if (_dbContext.Database.GetPendingMigrations().Count() > 0)
                {

                     _dbContext.Database.Migrate();
                }


                if (!_roleManager.RoleExistsAsync(SD.Role_Customer).GetAwaiter().GetResult())
                {
                    _roleManager.CreateAsync(new IdentityRole(SD.Role_Customer)).GetAwaiter().GetResult();  
                    _roleManager.CreateAsync(new IdentityRole(SD.Role_Admin)).GetAwaiter().GetResult();



                    _userManager.CreateAsync(new ApplicationUser
                    {
                        UserName = "admin@gmail.com",
                        Email = "admin@gmail.com",
                        Name = "Vlad",
                        StreetAdreess = "st.base120",
                        City = "Dnipro",
                        PostalCode = "111",

                    },"Admin123*").GetAwaiter().GetResult();


                    _userManager.CreateAsync(new ApplicationUser
                    {
                        UserName = "user@gmail.com",
                        Email = "user@gmail.com",
                        Name = "Test",
                        StreetAdreess = "st.base120",
                        City = "Kyiv",
                        PostalCode = "111",

                    }, "Admin123*").GetAwaiter().GetResult();

                    var admin = _dbContext.ApplicationUsers.FirstOrDefault(u=>u.Email == "admin@gmail.com");
                    var user = _dbContext.ApplicationUsers.FirstOrDefault(u => u.Email == "user@gmail.com");

                    _userManager.AddToRoleAsync(admin, SD.Role_Admin).GetAwaiter().GetResult();
                    _userManager.AddToRoleAsync(user, SD.Role_Customer).GetAwaiter().GetResult();

                }

                return;

            }
            catch (Exception ex) { }

        }
    }
}
