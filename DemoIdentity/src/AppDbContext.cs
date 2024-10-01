using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace src
{
    // If you want, you can also add additional properties to the above fields. 
    // To do that you need to create another class (for example, ApplicationUser) and inherit from the IdentityUser.
    // ASP.NET Core Identity API already implements IdentityDbContext class, which inherits from the DBContext class. 
    // It already includes the DbSet Properties for the Identity Models.
    public class AppDbContext : IdentityDbContext<IdentityUser, IdentityRole, string>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }
    }
}