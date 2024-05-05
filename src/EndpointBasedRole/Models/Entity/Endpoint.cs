namespace EndpointBasedRole.Models.Entity
{
    public class Endpoint
    {
        public Guid Id { get; set; }
        public bool IsActive { get; set; } = true;

        public string ActionType { get; set; }
        public string HttpType { get; set; }
        public string Defination { get; set; }
        public string Code { get; set; }

        public Guid MenuId { get; set; }
        public Menu Menu { get; set; }

        public List<Role> Roles { get; set; } = new();
    }
}
