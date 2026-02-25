using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using QuizCore.Application.Interfaces.Auth;
using QuizCore.Domain.Entities;

namespace QuizCore.Infrastructure.Services.Auth;

public class JwtTokenGenerator : ITokenGenerator
{
    private readonly IConfiguration _config;
    public JwtTokenGenerator(IConfiguration config) => _config = config;

    public string GenerateToken(User user, out int expiresIn)
    {
        var jwtSettings = _config.GetSection("Jwt");
        var secret = jwtSettings["Secret"] ?? "QuizCoreSuperSecretKey_GeneratedMustBeLongEnough123!";
        var expiration = int.Parse(jwtSettings["ExpirationInSeconds"] ?? "86400");
        expiresIn = expiration;

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.Username),
            new Claim(ClaimTypes.Role, user.Role.ToString())
        };

        var token = new JwtSecurityToken(
            issuer: jwtSettings["Issuer"],
            audience: jwtSettings["Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddSeconds(expiration),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
