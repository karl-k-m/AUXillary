using AUXillary.Data;
using AUXillary.DTO;
using AUXillary.Models;
using AUXillary.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AUXillary.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly AuthService _authService;
    private readonly ApiContext _apiContext;
    
    public AuthController(ApiContext apiContext, AuthService authService)
    {
        _apiContext = apiContext;
        _authService = authService;
    }

    /// <summary>
    /// Register a new user
    /// </summary>
    /// <param name="request">User registration details</param>
    /// <returns>Success message</returns>
    [HttpPost("register")]
    public async Task<ActionResult<User>> RegisterUser(UserRegisterDTO request)
    {
        var result = await _authService.RegisterUser(request);
        if (!result.Success)
        {
            return BadRequest(result.Message);
        }
        return Ok(result.Message);
    }
    
    /// <summary>
    /// Login a user
    /// </summary>
    /// <param name="request">User login details</param>
    /// <returns>Success message</returns>
    [HttpPost("login")]
    public async Task<ActionResult<User>> LoginUser(UserLoginDTO request)
    {
        var result = await _authService.LoginUser(request);
        if (!result.Success)
        {
            return BadRequest(result.Message);
        }
        return Ok(result.Message);
    }
    
    // Get all users (testing purposes)
    [HttpGet("users")]
    public async Task<ActionResult<IEnumerable<User>>> GetUsers()
    {
        return await _apiContext.Users.ToListAsync();
    }
}