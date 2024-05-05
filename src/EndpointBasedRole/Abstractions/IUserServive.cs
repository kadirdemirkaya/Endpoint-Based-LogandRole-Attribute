using EndpointBasedRole.Models;

namespace EndpointBasedRole.Services
{
    public interface IUserServive
    {
        Task<UserResponseModel> UserRegisterAsync(RegisterViewModel registerViewModel);

        Task<UserResponseModel> UserLoginAsync(LoginViewModel loginViewModel);

        Task<bool> AddRoleToUserAsync(Guid userId, string[] roles);

        Task<List<string>> GetRolesToUserAsync(Guid userId);

        Task<List<string>> GetRolesToUserAsync(string email);

        Task<bool> HasRolePermissionAsync(string email, string code);
    }
}
