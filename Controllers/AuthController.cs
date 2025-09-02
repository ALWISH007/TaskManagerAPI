using Microsoft.AspNetCore.Mvc;
using TaskManagerAPI.Models.Auth;
using TaskManagerAPI.Services.Auth;

namespace TaskManagerAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginModel model)
    {
        var token = await _authService.AuthenticateAsync(model.Username, model.Password);

        if (token == null)
        {
            return Unauthorized("Invalid username or password");
        }

        return Ok(new { Token = token }); // Return the token to the client
    }
}