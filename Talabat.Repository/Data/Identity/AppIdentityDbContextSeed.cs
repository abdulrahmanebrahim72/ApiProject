using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Talabat.Core.Entities.Identity;

namespace Talabat.Repository.Data.Identity
{
    public static class AppIdentityDbContextSeed
    {
        public static async Task SeedUsersAsync(UserManager<AppUser> _userManager)
        {
            if(_userManager.Users.Count() == 0)
            {
                var user = new AppUser
                {
                    DisplayName = "Abdulrahman Ebrahim",
                    Email = "Abdulrahmanebrahim72@gmail.com",
                    UserName = "Abdulrahmanebrahim72",
                    PhoneNumber = "01033225657"
                };

                await _userManager.CreateAsync(user, "Pa$$w0rd");
            }
        }
    }
}
