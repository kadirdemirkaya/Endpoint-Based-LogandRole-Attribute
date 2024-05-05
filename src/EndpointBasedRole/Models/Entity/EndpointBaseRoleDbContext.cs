using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace EndpointBasedRole.Models.Entity
{
    public class EndpointBaseRoleDbContext : IdentityDbContext<User, Role, Guid>
    {
        public EndpointBaseRoleDbContext()
        {

        }

        public EndpointBaseRoleDbContext(DbContextOptions options) : base(options)
        {

        }

        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<Menu> Menus { get; set; }
        public DbSet<Endpoint> Endpoints { get; set; }
    }
}
