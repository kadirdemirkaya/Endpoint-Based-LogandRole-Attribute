using EndpointBasedRole.Abstractions;
using EndpointBasedRole.Attributes;
using EndpointBasedRole.Models.Endpoints;
using EndpointBasedRole.Models.Entity;
using EndpointBasedRole.Models.Enums;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.EntityFrameworkCore;
using System;
using System.Reflection;

namespace EndpointBasedRole.Services
{
    public class EndpointService(IRepository<EndpointBasedRole.Models.Entity.Endpoint> _endpointRepository, IRepository<Menu> _menuRepository, RoleManager<Role> _roleManager) : IEndpointService
    {
        public List<MenuModel> GetAuthorizeDefinationEndpoint(Type type)
        {
            Assembly assembly = Assembly.GetAssembly(type);
            var controllers = assembly.GetTypes().Where(t => t.IsAssignableTo(typeof(ControllerBase)));

            List<MenuModel> menus = new();

            if (controllers is not null)
                foreach (var controller in controllers)
                {
                    var actions = controller.GetMethods().Where(m => m.IsDefined(typeof(AuthorizeDefinationAttribute)));
                    if (actions is not null)
                        foreach (var action in actions)
                        {
                            var attributes = action.GetCustomAttributes(true);
                            if (attributes is not null)
                            {
                                MenuModel menu = null;

                                var authorizeDefAttribute = attributes.FirstOrDefault(a => a.GetType() == typeof(AuthorizeDefinationAttribute)) as AuthorizeDefinationAttribute;

                                if (!menus.Any(m => m.Name == authorizeDefAttribute.Menu))
                                {
                                    menu = new() { Name = authorizeDefAttribute.Menu };
                                    menus.Add(menu);
                                }
                                else
                                    menu = menus.FirstOrDefault(m => m.Name == authorizeDefAttribute.Menu);

                                Models.Endpoints.ActionModel _action = new()
                                {
                                    Defination = authorizeDefAttribute.Defination,
                                    ActionType = Enum.GetName(typeof(ActionType), authorizeDefAttribute.ActionType),
                                };

                                var httpMethodAttribute = attributes.FirstOrDefault(a => a.GetType().IsAssignableTo(typeof(HttpMethodAttribute))) as HttpMethodAttribute;

                                if (httpMethodAttribute is not null)
                                    _action.HttpType = httpMethodAttribute.HttpMethods.FirstOrDefault();
                                else
                                    _action.HttpType = "UNDEFINED";

                                _action.Code = $"{_action.HttpType}_{_action.ActionType}_{_action.Defination.Trim().Replace(" ", "").Replace("-", "_")}";

                                menu.Actions.Add(_action);
                            }
                        }
                }
            return menus;
        }

        public List<LogModel> GetSpecificLogEndpoint(Type type)
        {
            Assembly assembly = Assembly.GetAssembly(type);
            var controllers = assembly.GetTypes().Where(t => t.IsAssignableTo(typeof(ControllerBase)));

            List<LogModel> logModels = new();

            if (controllers is not null)
                foreach (var controller in controllers)
                {
                    var actions = controller.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly)
                             .Where(m => m.IsDefined(typeof(SpecificLogAttribute), false))
                             .ToList();
                    if (actions is not null)
                        foreach (var action in actions)
                        {
                            var attributes = action.GetCustomAttributes(true);
                            if (attributes is not null)
                            {
                                var authorizeDefAttribute = attributes.FirstOrDefault(a => a.GetType() == typeof(SpecificLogAttribute)) as SpecificLogAttribute;

                                logModels.Add(new() { MenuName = authorizeDefAttribute.Menu, MethodName = action.Name, ActionType = Enum.GetName(typeof(ActionType), authorizeDefAttribute.ActionType) });
                            }
                        }
                }
            return logModels;
        }

        public async Task AssignRoleEndpointAsync(string[] roles, string code, string menu, Type type)
        {
            Menu _menu = await _menuRepository.GetAsync(m => m.Name == menu);
            if (_menu is null)
            {
                _menu = new() { Id = Guid.NewGuid(), Name = menu };
                await _menuRepository.AddAsync(_menu);
                await _endpointRepository.SaveChangesAsync();
            }

            var _endpoint = await _endpointRepository.Table.Include(e => e.Menu).Include(e => e.Roles).FirstOrDefaultAsync(e => e.Code == code && e.Menu.Name == menu);

            if (_endpoint is null)
            {
                var action = GetAuthorizeDefinationEndpoint(type)
                    .FirstOrDefault(m => m.Name == menu)?
                    .Actions.FirstOrDefault(e => e.Code == code);

                _endpoint = new()
                {
                    Id = Guid.NewGuid(),
                    Code = action.Code,
                    ActionType = action.ActionType,
                    Defination = action.Defination,
                    HttpType = action.HttpType,
                    MenuId = _menu.Id
                };

                await _endpointRepository.AddAsync(_endpoint);
                await _endpointRepository.SaveChangesAsync();
            }

            _endpoint.Roles.Clear();

            var existRoles = _roleManager.Roles.Where(r => roles.Contains(r.Name)).ToList();

            foreach (var role in existRoles)
                _endpoint.Roles.Add(role);

            await _endpointRepository.SaveChangesAsync();
        }

        public async Task<List<string>> GetRolesToEndpointAsync(string code, string menu)
        {
            EndpointBasedRole.Models.Entity.Endpoint? endpoint = await _endpointRepository.Table.Include(e => e.Roles).Include(e => e.Menu).FirstOrDefaultAsync(e => e.Code == code && e.Menu.Name == menu);

            if (endpoint is null)
                return null;

            return endpoint?.Roles.Select(r => r.Name).ToList();
        }
    }
}
