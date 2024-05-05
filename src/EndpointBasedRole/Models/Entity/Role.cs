using Microsoft.AspNetCore.Identity;

namespace EndpointBasedRole.Models.Entity
{
    public class Role : IdentityRole<Guid>
    {
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? ChangedAt { get; set; }
        public bool IsActive { get; set; } = true;


        public List<Endpoint> Endpoints { get; set; }
    }
}
