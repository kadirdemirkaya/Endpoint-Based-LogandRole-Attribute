using EndpointBasedRole.Api.Models;
using EndpointBasedRole.Attributes;
using EndpointBasedRole.Models.Enums;
using EndpointBasedRole.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using static EndpointBasedRole.Constants.Constant;

namespace EndpointBasedRole.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [AllowAnonymous]
    public class RoleController(IRoleService _roleService) : BaseController
    {
        [HttpGet]
        [AuthorizeDefination(Menu = AuthorizeDefination.Role, ActionType = ActionType.Reading, Defination = "get-roles")]
        [Route("get-roles")]
        public async Task<IActionResult> GetRoles()
        {
            return Ok(_roleService.GetAllRoles());
        }

        [HttpGet]
        [AuthorizeDefination(Menu = AuthorizeDefination.Role, ActionType = ActionType.Reading, Defination = "get-roles-by-id")]
        [Route("get-roles-by-id")]
        public async Task<IActionResult> GetRolesById([FromHeader] Guid id)
        {
            var res = await _roleService.GetRoleByIdAsync(id);
            return Ok(new RoleViewModel() { Id = res.id, Name = res.name });
        }

        [HttpPost]
        //[AuthorizeDefination(Menu = AuthorizeDefination.Role, ActionType = ActionType.Writing, Defination = "create-role")]
        [Route("create-role")]
        public async Task<IActionResult> CreateRole([FromQuery] string name)
        {
            return Ok(await _roleService.CreateRoleAsync(name));
        }

        [HttpPut]
        [AuthorizeDefination(Menu = AuthorizeDefination.Role, ActionType = ActionType.Updating, Defination = "update-role")]
        [Route("update-role")]
        public async Task<IActionResult> UpdateRole([FromHeader] Guid id, [FromQuery] string name)
        {
            return Ok(await _roleService.UpdateRoleAsync(id, name));
        }


        [HttpDelete]
        [AuthorizeDefination(Menu = AuthorizeDefination.Role, ActionType = ActionType.Deleting, Defination = "delete-role")]
        [Route("delete-role")]
        public async Task<IActionResult> DeleteRole([FromHeader] Guid id)
        {
            return Ok(await _roleService.DeleteRoleAsync(id));
        }
    }
}
