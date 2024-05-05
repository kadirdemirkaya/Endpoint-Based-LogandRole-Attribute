using EndpointBasedRole.Abstractions;
using EndpointBasedRole.Models;
using EndpointBasedRole.Models.Entity;
using EndpointBasedRole.Constants;
using Microsoft.AspNetCore.Identity;
using EndpointBasedRole.Services;
using Microsoft.EntityFrameworkCore;

namespace EndpointBasedRole.Services
{
    public class UserService(UserManager<User> userManager, ITokenService tokenService, IRepository<Endpoint> _endpointRepository) : IUserServive
    {
        private readonly UserManager<User> _userManager = userManager;
        private readonly ITokenService _tokenService = tokenService;

        public async Task<UserResponseModel> UserLoginAsync(LoginViewModel loginViewModel)
        {
            if (loginViewModel is null)
                throw new NullReferenceException($"{nameof(LoginViewModel)} is null !");

            User? user = await _userManager.FindByEmailAsync(loginViewModel.Email);

            if (user is null)
                return new()
                {
                    IsSuccess = false,
                    Errors = new string[] { "User Not found" }
                };

            bool result = await _userManager.CheckPasswordAsync(user, loginViewModel.Password);

            if (!result)
                return new()
                {
                    IsSuccess = false,
                    Errors = new string[] { "Invalid user inputs" }
                };

            var token = _tokenService.GenerateToken(user.Email, user.Id.ToString());

            return new()
            {
                IsSuccess = true,
                Token = token.AccessToken
            };
        }

        public async Task<UserResponseModel> UserRegisterAsync(RegisterViewModel registerViewModel)
        {
            if (registerViewModel is null)
                throw new NullReferenceException($"{nameof(RegisterViewModel)} is null !");

            User user = User.Create(registerViewModel.Name, registerViewModel.Email, registerViewModel.AboutMe, registerViewModel.PhoneNumber);

            IdentityResult result = await _userManager.CreateAsync(user, registerViewModel.Password);

            if (result.Succeeded)
            {
                IdentityResult? roleResult = await _userManager.AddToRoleAsync(user, Constant.Role.User);
                return roleResult.Succeeded is true ? new() { IsSuccess = true } : new() { IsSuccess = false };
            }
            else
            {
                string[] errorMessages = result.Errors.Select(e => e.Description).ToArray();
                return new()
                {
                    IsSuccess = false,
                    Errors = errorMessages
                };
            }
        }

        public async Task<bool> AddRoleToUserAsync(Guid userId, string[] roles)
        {
            EndpointBasedRole.Models.Entity.User? user = await _userManager.FindByIdAsync(userId.ToString());
            var userRoles = await _userManager.GetRolesAsync(user);
            IdentityResult identityResult = await _userManager.RemoveFromRolesAsync(user, userRoles);
            if (identityResult.Succeeded)
            {
                IdentityResult idenRes = await _userManager.AddToRolesAsync(user, roles);
                return idenRes.Succeeded;
            }
            return false;
        }

        public async Task<List<string>> GetRolesToUserAsync(Guid userId)
        {
            EndpointBasedRole.Models.Entity.User? user = await _userManager.FindByIdAsync(userId.ToString());
            var userRoles = await _userManager.GetRolesAsync(user);
            if (userRoles.Count() == 0)
                return null;
            return userRoles.ToList();
        }

        public async Task<List<string>> GetRolesToUserAsync(string email)
        {
            EndpointBasedRole.Models.Entity.User? user = await _userManager.FindByEmailAsync(email);
            var userRoles = await _userManager.GetRolesAsync(user);
            if (userRoles.Count() == 0)
                return null;
            return userRoles.ToList();
        }

        public async Task<bool> HasRolePermissionAsync(string email, string code)
        {
            var userRoles = await GetRolesToUserAsync(email);

            if (!userRoles.Any())
                return false;

            Endpoint? endpoint = await _endpointRepository
                .Table
                .Include(e => e.Roles)
                .FirstOrDefaultAsync(r => r.Code == code);

            if (endpoint is null)
                return false;

            var endpointRoles = endpoint.Roles.Select(r => r.Name);

            foreach (var userRole in userRoles)
                foreach (var endpointRole in endpointRoles)
                    if (userRole == endpointRole)
                        return true;
            return false;
        }
    }
}
