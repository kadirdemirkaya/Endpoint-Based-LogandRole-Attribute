using EndpointBasedRole.Abstractions;
using EndpointBasedRole.Api.Services;
using EndpointBasedRole.Extensions;
using EndpointBasedRole.Options;
using EndpointBasedRole.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Text;

namespace EndpointBasedRole
{
    public static class ServiceRegistration
    {
        public static IServiceCollection EndpointBasedRoleServiceRegistration(this IServiceCollection services)
        {
            var jwtOptions = services.GetOptions<JwtOptions>("JwtOptions");

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultSignInScheme = JwtBearerDefaults.AuthenticationScheme;
            })
               .AddJwtBearer(options =>
               {
                   options.SaveToken = true;
                   options.RequireHttpsMetadata = false;
                   options.TokenValidationParameters = new TokenValidationParameters()
                   {
                       ValidateIssuer = true,
                       ValidateAudience = true,
                       ValidAudience = jwtOptions.Audience,
                       ValidIssuer = jwtOptions.Issuer,
                       NameClaimType = ClaimTypes.Email,
                       IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.Secret)),
                   };
               });

            services.AddScoped<ITokenService, TokenService>();

            services.AddScoped<IEndpointService, EndpointService>();

            services.AddScoped(typeof(IRepository<>), typeof(Repository<>));

            services.AddScoped<IUserServive, UserService>();

            services.AddScoped<IRoleService, RoleService>();

            return services;
        }
    }
}
