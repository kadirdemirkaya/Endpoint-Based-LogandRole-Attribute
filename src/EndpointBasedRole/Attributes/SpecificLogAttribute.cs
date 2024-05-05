using EndpointBasedRole.Models.Enums;

namespace EndpointBasedRole.Attributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false)]
    public class SpecificLogAttribute : Attribute
    {
        public string Menu { get; set; }
        public ActionType ActionType { get; set; }
    }
}
