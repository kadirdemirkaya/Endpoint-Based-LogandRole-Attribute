namespace EndpointBasedRole.Models
{
    public class RoleAddToUserViewModel
    {
        public Guid UserId { get; set; }
        public string[] Roles { get; set; }
    }
}
