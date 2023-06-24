using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Talabat.Core.Entities.Identity;

namespace Talabat.Repository.Identity
{
    public class AppIdentityDbContextSeed
    {
        public static async Task SeedUsersAsync(UserManager<AppUser> userManager)
        {
            if(!userManager.Users.Any())
            {
                var user = new AppUser()
                {
                    DisplayName = "Hossam Mostafa",
                    Email = "hossam@gmail.com",
                    UserName = "Hossam.Mostafa",
                    PhoneNumber = "01000135683",
                };
                await userManager.CreateAsync(user , "Pa$$w0rd");
            }
        }
    }
}
