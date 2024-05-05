﻿using EndpointBasedRole.Abstractions;
using EndpointBasedRole.Extensions;
using EndpointBasedRole.Models;
using EndpointBasedRole.Options;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Reflection.Metadata;
using System.Security.Claims;
using System.Text;

namespace EndpointBasedRole.Services
{
    public class TokenService(IConfiguration configuration) : ITokenService
    {
        public Token GenerateToken(string email, string id, string? role = null)
        {
            JwtOptions jwtOptions = configuration.GetOptions<JwtOptions>("JwtOptions");

            var siginingCredentials = new SigningCredentials(
                new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.Secret)),
                SecurityAlgorithms.HmacSha256
            );

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub,Guid.NewGuid().ToString()),
                new Claim("Id", id),
                new Claim(JwtRegisteredClaimNames.GivenName,email),
                new Claim(JwtRegisteredClaimNames.Email,email),
                new Claim("Email",email),
                new Claim(ClaimTypes.Role,role ?? EndpointBasedRole.Constants.Constant.Role.User),
                new Claim(ClaimTypes.Name,email),
                new Claim(JwtRegisteredClaimNames.Jti,Guid.NewGuid().ToString())
            };

            var _expries = DateTime.Now.AddMinutes(int.Parse(jwtOptions.ExpiryMinutes));

            var securityToken = new JwtSecurityToken(
                issuer: jwtOptions.Issuer,
                audience: jwtOptions.Audience,
                expires: _expries,
                claims: claims,
                signingCredentials: siginingCredentials
            );

            Token token = new();
            token.Expiration = _expries;
            token.AccessToken = new JwtSecurityTokenHandler().WriteToken(securityToken);

            return token;
        }
    }
}
