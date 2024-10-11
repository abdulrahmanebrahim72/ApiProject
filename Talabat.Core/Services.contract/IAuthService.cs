using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Talabat.Core.Entities.Identity;

namespace Talabat.Core.Services.contract
{
    public interface IAuthService
    {
        Task<string> CreateTokenAsynk(AppUser user, UserManager<AppUser> userManager);
    }
}
