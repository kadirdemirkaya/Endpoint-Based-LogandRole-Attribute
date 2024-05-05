using EndpointBasedRole.Attributes;
using EndpointBasedRole.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.Routing;
using System.Reflection;

namespace EndpointBasedRole.Filters
{
    public class RolePermissionFilter(IUserServive _userServive) : IAsyncActionFilter
    {
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var name = context.HttpContext.User.Identity?.Name;
            if (!string.IsNullOrEmpty(name))
            {
                var descriptor = context.ActionDescriptor as ControllerActionDescriptor;

                var authorizeDefinationAttribute = descriptor.MethodInfo.GetCustomAttribute(typeof(AuthorizeDefinationAttribute)) as AuthorizeDefinationAttribute;

                var httpMethodAttribute = descriptor.MethodInfo.GetCustomAttribute(typeof(HttpMethodAttribute)) as HttpMethodAttribute;

                var code = $"{httpMethodAttribute.HttpMethods.First()}_{authorizeDefinationAttribute.ActionType}_{authorizeDefinationAttribute.Defination.Trim().Replace(" ", "").Replace("-", "_")}";

                var hasRole = await _userServive.HasRolePermissionAsync(name, code);

                if (!hasRole)
                    context.Result = new UnauthorizedResult();
                else
                    await next();
            }
            else
                await next();
        }
    }
}
