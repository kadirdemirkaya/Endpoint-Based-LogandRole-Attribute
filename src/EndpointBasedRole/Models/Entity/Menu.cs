namespace EndpointBasedRole.Models.Entity
{
    public class Menu
    {
        public Guid Id { get; set; }
        public bool IsActive { get; set; } = true;
        public string Name { get; set; }
        public List<Endpoint> Endpoints { get; set; }
    }
}
