using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using TaskManagerAPI.Models.Auth;

namespace TaskManagerAPI.Services.Auth;

public class SimpleAuthService : IAuthService
{
    private readonly IConfiguration _configuration;

    // In a real app, you would check against a database of users.
    // This is a hardcoded user for demonstration.
    private readonly string _hardcodedUsername = "admin";//Makes the fields immutable after initialization(either here or in constructor)
    private readonly string _hardcodedPassword = "password"; // NEVER do this in production.

    public SimpleAuthService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public async Task<string?> AuthenticateAsync(string username, string password)
    {
        // Simulate a small async delay
        await Task.Delay(100);

        // 1. Validate credentials (This is the part you replace with a database call)
        if (username != _hardcodedUsername || password != _hardcodedPassword)
        {
            return null; // Authentication failed
        }

        // 2. Authentication successful, so generate a JWT token
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(_configuration["Jwt:Key"]!);//That converts your JWT secret string into a byte array so it can be used by the signing algorithm.

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.Name, username),
                // You can add more claims here (e.g., roles, user ID)
                // new Claim(ClaimTypes.Role, "Administrator"),
            }),
            Expires = DateTime.UtcNow.AddHours(1), // Token expires in 1 hour
            Issuer = _configuration["Jwt:Issuer"],
            Audience = _configuration["Jwt:Audience"],
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }
}