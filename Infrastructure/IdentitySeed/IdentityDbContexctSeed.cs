using Domain.Entities;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.IdentitySeed
{
    public static class IdentityDbContexctSeed
    {
        public static async Task SeedUserAsync(UserManager<AppIdentityUser> userManager)
        {
            if (!userManager.Users.Any())
            {
                var user = new AppIdentityUser()
                {
                    DisplayName="MohamedAbdelaal",
                    Email = "Mohamed.Abdelaal@gmail.com",
                    UserName = "MohamedAbdelaal",
                    PhoneNumber = "01011470221"
                };
                await userManager.CreateAsync(user, "P##E0p$$");
            }
        }
    }
}
