using EndpointBasedRole.Models.Endpoints;
using EndpointBasedRole.Models.Entity;
using Microsoft.AspNetCore.Mvc;

namespace EndpointBasedRole.Abstractions
{
    public interface IEndpointService
    {
        List<MenuModel> GetAuthorizeDefinationEndpoint(Type type);

        List<LogModel> GetSpecificLogEndpoint(Type type);

        public Task AssignRoleEndpointAsync(string[] roles, string code, string menu, Type type);

        public Task<List<string>> GetRolesToEndpointAsync(string code, string menu);
    }
}
