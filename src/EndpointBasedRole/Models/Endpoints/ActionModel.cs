using EndpointBasedRole.Models.Enums;

namespace EndpointBasedRole.Models.Endpoints
{
    public class ActionModel
    {
        public string ActionType { get; set; }
        public string HttpType { get; set; }
        public string Defination { get; set; }
        public string? Code { get; set; }
    }
}
