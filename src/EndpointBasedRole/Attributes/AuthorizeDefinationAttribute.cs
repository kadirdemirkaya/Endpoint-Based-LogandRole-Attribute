using EndpointBasedRole.Models.Enums;

namespace EndpointBasedRole.Attributes
{
    public class AuthorizeDefinationAttribute : Attribute
    {
        public string Menu { get; set; }
        public string Defination { get; set; }
        public ActionType ActionType { get; set; }
    }
}
