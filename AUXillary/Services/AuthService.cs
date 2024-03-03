using System.ComponentModel.DataAnnotations;
using System.Security.Cryptography;
using AUXillary.Data;
using AUXillary.DTO;
using AUXillary.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;

namespace AUXillary.Services;

public class AuthService
{
    private readonly ApiContext _apiContext;
    
    public AuthService(ApiContext apiContext)
    {
        _apiContext = apiContext;
    }
    
    /// <summary>
    /// Register a new user
    /// </summary>
    /// <param name="request">User registration details</param>
    /// <returns>Success message</returns>
    public async Task<(bool Success, string Message)> RegisterUser(UserRegisterDTO request)
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

        // Check if user exists
        if (await UserExists(request.Username))
        {
            return (false, "Username is already taken");
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
        
        // Create a new user
        var user = new User
        {
            Username = request.Username,
            Email = request.Email
        };
        
        // Generate a random salt and create a password hash
        user.PasswordSalt = await GenerateSalt();
        user.PasswordHash = await CreatePasswordHash(request.Password, user.PasswordSalt);
        
        _apiContext.Users.Add(user);
        await _apiContext.SaveChangesAsync();
        
        return (true, "User registered successfully.");
    }
    
    public Task<(bool Success, string Message, string AccessToken, string RefreshToken)> LoginUser(UserLoginDTO request)
    {
        // Request validation
        // Make sure username and password are not empty
        if (string.IsNullOrEmpty(request.Username) || string.IsNullOrEmpty(request.Password))
        {
            return Task.FromResult((false, "Username and password are required", string.Empty, string.Empty));
        }
        
        // Check if user exists
        var user = _apiContext.Users.FirstOrDefault(x => x.Username == request.Username);
        if (user == null)
        {
            return Task.FromResult((false, "Invalid username or password", string.Empty ,string.Empty));
        }
        
        // Check if password is correct
        var passwordHash = CreatePasswordHash(request.Password, user.PasswordSalt).Result;
        if (!passwordHash.SequenceEqual(user.PasswordHash))
        {
            return Task.FromResult((false, "Invalid username or password", string.Empty, string.Empty));
        }
        
        // Generate access and refresh tokens
        var accessToken = GenerateToken().Result;
        var refreshToken = GenerateToken().Result;
        
        return Task.FromResult((true, "User logged in successfully", accessToken, refreshToken));
    }
    
    /// <summary>
    /// Check if a user with the given username exists
    /// </summary>
    /// <param name="username">Username to check</param>
    /// <returns>True if user exists, false otherwise</returns>
    private async Task<bool> UserExists(string username)
    {
        return await _apiContext.Users.AnyAsync(x => x.Username == username);
    }
    
    /// <summary>
    /// Create a password hash using PBKDF2
    /// </summary>
    /// <param name="password">Password to hash</param>
    /// <param name="passwordSalt">Salt used in hash creation</param>
    /// <param name="iterations">Work factor</param>
    /// <param name="hashByteSize">Size of the hash</param>
    /// <returns>Hashed password</returns>
    private async Task<byte[]> CreatePasswordHash(string password, byte[] passwordSalt, int iterations = 10000, int hashByteSize = 20)
    {
        using (var pbkdf2 = new Rfc2898DeriveBytes(password, passwordSalt, iterations, HashAlgorithmName.SHA3_512))
        {
            return pbkdf2.GetBytes(hashByteSize);
        }
    }
    
    /// <summary>
    /// Generate a random salt
    /// </summary>
    /// <param name="size">Size of the salt</param>
    /// <returns>Random salt</returns>
    private async Task<byte[]> GenerateSalt(int size = 16)
    {
        using (var randomNumberGenerator = RandomNumberGenerator.Create())
        {
            var randomNumber = new byte[size];
            randomNumberGenerator.GetBytes(randomNumber);
            return randomNumber;
        }
    }

    /// <summary>
    /// Generate a random token
    /// </summary>
    /// <param name="size">Size of the token</param>
    /// <returns>Random token</returns>
    private async Task<string> GenerateToken(int size = 16)
    {
        using (var randomNumberGenerator = RandomNumberGenerator.Create())
        {
            var randomNumber = new byte[size];
            randomNumberGenerator.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }
    }
}