using EndpointBasedRole.Models;
using EndpointBasedRole.Models.Entity;
using EndpointBasedRole.Services;
using EndpointBasedRole.Attributes;
using EndpointBasedRole.Models.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using static EndpointBasedRole.Constants.Constant;
using Microsoft.EntityFrameworkCore;
using EndpointBasedRole.Api.Filters;

namespace EndpointBasedRole.Api.Controllers
{
    public class AuthController(IUserServive _userServive, UserManager<User> _userManager) : BaseController
    {
        [HttpPost]
        [AllowAnonymous]
        [Route("user-register")]
        public async Task<IActionResult> Register([FromBody] RegisterViewModel registerViewModel)
        {
            UserResponseModel userResponseModel = await _userServive.UserRegisterAsync(registerViewModel);

            return Ok(userResponseModel);
        }

        [HttpPost]
        [AllowAnonymous]
        [Route("user-login")]
        public async Task<IActionResult> Login([FromBody] LoginViewModel loginViewModel)
        {
            UserResponseModel? userResponseModel = await _userServive.UserLoginAsync(loginViewModel);

            return Ok(userResponseModel);
        }


        [HttpGet]
        [Route("get-user")]
        [TypeFilter(typeof(SpecificLogFilter))]
        [SpecificLog(Menu = AuthorizeDefination.Auth, ActionType = ActionType.Reading)]
        [AuthorizeDefination(Menu = AuthorizeDefination.Auth, ActionType = ActionType.Reading, Defination = "get-user")]
        public async Task<IActionResult> GetUser([FromQuery] string email)
        {
            return Ok(await _userManager.FindByEmailAsync(email));
        }

        [HttpPost]
        [Route("update-user-password")]
        [AuthorizeDefination(Menu = AuthorizeDefination.Auth, ActionType = ActionType.Updating, Defination = "update-user-password")]
        public async Task<IActionResult> GetUser([FromQuery] string email, [FromQuery] string oldPassword, [FromQuery] string newPassword)
        {
            var user = await _userManager.FindByEmailAsync(email);
            var identityResult = await _userManager.ChangePasswordAsync(user, oldPassword, newPassword);
            return Ok(identityResult.Succeeded);
        }

        [HttpDelete]
        [Route("delete-user")]
        [SpecificLog(Menu = AuthorizeDefination.Auth, ActionType = ActionType.Deleting)]
        [AuthorizeDefination(Menu = AuthorizeDefination.Auth, ActionType = ActionType.Deleting, Defination = "delete-user")]
        public async Task<IActionResult> DeleteUser([FromHeader] string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            var identityResult = await _userManager.DeleteAsync(user);
            return Ok(identityResult.Succeeded);
        }

        [HttpGet]
        [Route("get-all-user")]
        [TypeFilter(typeof(SpecificLogFilter))]
        [SpecificLog(Menu = AuthorizeDefination.Auth, ActionType = ActionType.Reading)]
        [AuthorizeDefination(Menu = AuthorizeDefination.Auth, ActionType = ActionType.Reading, Defination = "get-all-user")]
        public async Task<IActionResult> GetAllUser()
        {
            return Ok(await _userManager.Users.ToListAsync());
        }

        [HttpPut]
        [AllowAnonymous]
        [Route("user-assign-role")]
        //[AuthorizeDefination(Menu = AuthorizeDefination.Auth, ActionType = ActionType.Updating, Defination = "user-assign-role")]
        public async Task<IActionResult> UserAssignRole([FromBody] RoleAddToUserViewModel roleAddToUserViewModel)
        {
            return Ok(await _userServive.AddRoleToUserAsync(roleAddToUserViewModel.UserId, roleAddToUserViewModel.Roles));
        }

        [HttpGet]
        [Route("user-get-roles")]
        [AuthorizeDefination(Menu = AuthorizeDefination.Auth, ActionType = ActionType.Reading, Defination = "user-get-roles")]
        public async Task<IActionResult> UserGetRoles([FromHeader] Guid userId)
        {
            return Ok(await _userServive.GetRolesToUserAsync(userId));
        }
    }
}
