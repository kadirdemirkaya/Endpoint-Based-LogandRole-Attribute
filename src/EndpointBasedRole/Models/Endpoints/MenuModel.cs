namespace EndpointBasedRole.Models.Endpoints
{
    public class MenuModel
    {
        public string Name { get; set; }
        public List<ActionModel> Actions { get; set; } = new();
    }
}
