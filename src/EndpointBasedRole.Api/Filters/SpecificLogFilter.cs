using EndpointBasedRole.Abstractions;
using EndpointBasedRole.Attributes;
using EndpointBasedRole.Models.Endpoints;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.Routing;
using System.Reflection;

namespace EndpointBasedRole.Api.Filters
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false)]
    public class SpecificLogFilter(IEndpointService _endpointService, ILogger<SpecificLogFilter> _logger) : Attribute, IAsyncActionFilter
    {
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var name = context.HttpContext.User.Identity?.Name;
            if (!string.IsNullOrEmpty(name))
            {
                var descriptor = context.ActionDescriptor as ControllerActionDescriptor;

                var specificLogAttribute = descriptor.MethodInfo.GetCustomAttribute(typeof(SpecificLogAttribute)) as SpecificLogAttribute;

                var httpMethodAttribute = descriptor.MethodInfo.GetCustomAttribute(typeof(HttpMethodAttribute)) as HttpMethodAttribute;

                var httpContext = context.HttpContext;
                var routeData = context.RouteData;

                if (routeData != null)
                {
                    string controllerName = routeData.Values["controller"]?.ToString();
                    string actionName = routeData.Values["action"]?.ToString();

                    List<LogModel>? logModels = _endpointService.GetSpecificLogEndpoint(typeof(Program));

                    foreach (var logModel in logModels)
                        if (logModel.MethodName == actionName && logModel.MenuName == controllerName)
                        {
                            _logger.LogCritical($"[{name}] user made a [{logModel.ActionType}] process in [{controllerName}] and in [{actionName}]");
                        }
                    await next();
                }
                else
                    await next();
            }
            else
                await next();
        }
    }
}
