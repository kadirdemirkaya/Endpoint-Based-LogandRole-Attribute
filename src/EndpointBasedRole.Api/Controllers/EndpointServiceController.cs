using EndpointBasedRole.Abstractions;
using EndpointBasedRole.Api.Models;
using EndpointBasedRole.Attributes;
using EndpointBasedRole.Models.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using static EndpointBasedRole.Constants.Constant;

namespace EndpointBasedRole.Api.Controllers
{
    public class EndpointServiceController(IEndpointService _endpointService) : BaseController
    {
        [HttpGet]
        [Route("get-authorize-defination-endpoints")]
        [AuthorizeDefination(Menu = AuthorizeDefination.Endpoint, ActionType = ActionType.Reading, Defination = "get-authorize-defination-endpoints")]
        [AllowAnonymous]
        public async Task<IActionResult> GetAuthorizeDefinationEndpoints()
        {
            return Ok(_endpointService.GetAuthorizeDefinationEndpoint(typeof(Program)));
        }

        [HttpPost]
        [Route("assign-role-to-endpoint")]
        [AuthorizeDefination(Menu = AuthorizeDefination.Endpoint, ActionType = ActionType.Writing, Defination = "assign-role-to-endpoint")]
        [AllowAnonymous]
        public async Task<IActionResult> AssignRoleToEndpoint([FromBody] AssignRoleEndpointViewModel assignRoleEndpointViewModel)
        {
            await _endpointService.AssignRoleEndpointAsync(assignRoleEndpointViewModel.Roles, assignRoleEndpointViewModel.EndpointCode, assignRoleEndpointViewModel.Menu, typeof(Program));
            return Ok(true);
        }


        [HttpPost]
        [Route("get-roles-to-endpoint")]
        [AuthorizeDefination(Menu = AuthorizeDefination.Endpoint, ActionType = ActionType.Reading, Defination = "get-roles-to-endpoint")]
        [AllowAnonymous]
        public async Task<IActionResult> GetRolesToEndpoints([FromQuery] string code, [FromQuery] string menu)
        {
            return Ok(await _endpointService.GetRolesToEndpointAsync(code, menu));
        }
    }
}
