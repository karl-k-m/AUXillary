using System.ComponentModel.DataAnnotations;
using AUXillary.DTO;
using AUXillary.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace AUXillary.Services;

public class AuthService
{
    public async Task<(bool Success, string Message)> Register(UserRegisterDTO request)
    {
        // Request validation
        // Make sure username and password are not empty
        if (string.IsNullOrEmpty(request.Username) || string.IsNullOrEmpty(request.Password))
        {
            return (false, "Username and password are required");
        }
        
        // Make sure email is not empty and is a valid email
        if (string.IsNullOrEmpty(request.Email) || !new EmailAddressAttribute().IsValid(request.Email))
        {
            return (false, "Email is required and must be a valid email address");
        }

        // TODO: Check if user exists
        if (await UserExists(request.Username))
        {
            return (false, "Username already exists");
        }
        
        // Check if password and confirm password match
        if (request.Password != request.ConfirmPassword)
        {
            return (false, "Password and confirm password do not match");
        }
        
        // Check if password is at least 8 characters long
        if (request.Password.Length < 8)
        {
            return (false, "Password must be at least 8 characters long");
        }
        
        // Create new user
        var user = new User
        {
            Username = request.Username
        };
    }
    
    public async Task<ActionResult> Login(UserLoginDTO request)
    {
        
    }
    
    public async Task<bool> UserExists(string username)
    {
        return false;
    }
}