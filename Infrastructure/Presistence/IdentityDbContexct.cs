using Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Presistence
{
    public class IdentityDbContexct : IdentityDbContext<AppIdentityUser>
    {
        public IdentityDbContexct(DbContextOptions<IdentityDbContexct> Context) : base(Context)
        {

        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
            base.OnModelCreating(builder);
        }
        public DbSet<RefreshToken> RefreshTokens { get; set; }
    }
}
