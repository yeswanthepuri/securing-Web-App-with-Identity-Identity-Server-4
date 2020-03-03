using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace identityExample.Data
{
    public class AppDbContext : IdentityDbContext 
    {
        ///Contsines all the tables related to identity
        public AppDbContext(DbContextOptions<AppDbContext> options)
        :base(options)
        {
            
        }
    }
}