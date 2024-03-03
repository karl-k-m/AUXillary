using AUXillary.DTO;
using AUXillary.Models;
using AUXillary.Services;
using Microsoft.AspNetCore.Mvc;

namespace AUXillary.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    public AuthController()
    {
        AuthService _authService = new AuthService();
    }
    
    public static User user = new User();

    // Register new user
    [HttpPost("register")]
    public async Task<ActionResult<User>> Register(UserRegisterDTO request)
    {
        return Ok();
    }
}