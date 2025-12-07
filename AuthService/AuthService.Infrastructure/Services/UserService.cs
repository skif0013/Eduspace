using AuthService.Application.DTOs;
using AuthService.Application.Interfaces;
using AuthService.Application.Interfaces.Services;
using AuthService.Domain.Entities;
using AuthService.Domain.Results;
using AuthService.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Shared.Messages;

namespace AuthService.Infrastructure.Services;

public class UserService : IUserService
{
    
    private readonly UserManager<User> _userManager;
    private readonly RoleManager<RoleIdentity> _roleManager;
    private readonly ITokenService _tokenService;
    private readonly IMessageService _messageService;

    public UserService(
        UserManager<User> userManager,
        RoleManager<RoleIdentity> roleManager,
        ITokenService tokenService,
        IMessageService messageService
    )
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _tokenService = tokenService;
        _messageService = messageService;
    }
    
    public async Task<Result<string>> RegisterAsync(CreateUserDto createUserDto)
    {
        var user = new User
        {
            UserName = createUserDto.UserName,
            Email = createUserDto.Email,
        };
        
        var result = await _userManager.CreateAsync(user, createUserDto.Password);
        if (!result.Succeeded)
        {
            var errors = string.Join(", ", result.Errors.Select(e => e.Description));
            return Result<string>.Failure($"Error creating user: {errors}");
        }

        var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
        Console.WriteLine(token);
        
        /*using var client = new HttpClient();
        string url = "http://emailservice:5003/api/Email/verify";
        string json = $@"{{
            ""to"": ""{request.Email}"",
            ""code"": ""{token}""
        }}";*/

        //var content = new StringContent(json, Encoding.UTF8, "application/json");

        //var response = await client.PostAsync(url, content);
        
        var confirmEmailEvent = new EmailVerifyEvent()
        {
            To = user.Email,
            UserName = user.UserName,
            VerificationLink = "",
            Code = token
        };
        await _messageService.SendMessageAsync("email:verify", confirmEmailEvent);
        
        return Result<string>.Success("user created successfully");
    }

    public async Task<Result<AuthResponse>> AuthenticateAsync(AuthRequest request)
    {
        var user = await _userManager.Users.FirstOrDefaultAsync(u => u.Email == request.Email);
        if (user == null)
        {
            return Result<AuthResponse>.Failure("Invalid Email");
        }

        var isEmailConfirmed = await _userManager.IsEmailConfirmedAsync(user);
        if (!isEmailConfirmed)
        {
            return Result<AuthResponse>.Failure("Email not confirmed");
        }

        var isPasswordCorrect = await _userManager.CheckPasswordAsync(user, request.Password);
        if (!isPasswordCorrect)
        {
            return Result<AuthResponse>.Failure("Invalid Password");
        }
        
        var userDto = new UserDto
        {
            Id = user.Id,
            UserName = user.UserName,
            Email = user.Email
        };
        
        var tokens = await _tokenService.CreateAccessTokenAsync(userDto);
        
        return Result<AuthResponse>.Success(new AuthResponse
        {
            Token = tokens.AccessToken
        });
    }
    
    public async Task<Result<string>> ForgotPasswordAsync(ForgotPasswordRequest request)
    {
        var user = await _userManager.FindByEmailAsync(request.Email);
        if (user == null)
        {
            return Result<string>.Failure($"{request.Email} - this email address is not registered");
        }
        
        var token = await _userManager.GeneratePasswordResetTokenAsync(user);
        Console.WriteLine(token);
        
        return Result<string>.Success("Password reset successfully");
    }

    public async Task<Result<string>> ResetPasswordAsync(ResetPasswordDto request)
    {
        var user = await _userManager.FindByEmailAsync(request.Email);
        if (user == null)
        {
            return Result<string>.Failure($"{request.Email} - this email address is not registered");
        }
        
        var resetPassResult = await _userManager.ResetPasswordAsync(user, request.Token, request.NewPassword!);

        if(!resetPassResult.Succeeded)
        {
            var errors = resetPassResult.Errors.Select(e => e.Description);
            return Result<string>.Failure(errors.ToString()!);
        }
        
        return Result<string>.Success("Password reset successfully");
    }

    public async Task<Result<string>> ConfirmEmailAsync(string email, string token)
    {
        var user = await _userManager.FindByEmailAsync(email);
        if (user == null)
        {
            return Result<string>.Failure("Invalid email address");
        }
        await _userManager.ConfirmEmailAsync(user, token);
        
        var roleExists = _roleManager.FindByNameAsync("User".ToString());
        if (!roleExists.IsCompletedSuccessfully)
        {
            Console.WriteLine("Role doesn't exist");
        }

        await _userManager.AddToRoleAsync(user, roleExists.Result.Name.ToString());
        
        var message = new CreateUserDTO()
        {
            UserId = user.Id,
            UserName = user.UserName,
            Email = user.Email,
            EventType = "user:created"
        };
        await _messageService.SendMessageAsync("user:created", message);
        
        return Result<string>.Success("Email confirmed");
    }
    
    public async Task<Result<string>> UpdateUserAsync(UpdateUserDTO userDto)
    {
        var user = await _userManager.FindByIdAsync(userDto.Id.ToString());
        if (user == null)
        {
            return Result<string>.Failure("User not found");
        }

        user.UserName = userDto.UserName;
        user.Email = userDto.Email;
        user.LastModified = DateTime.UtcNow;

        var result = await _userManager.UpdateAsync(user);
        if (!result.Succeeded)
        {
            var errors = string.Join(", ", result.Errors.Select(e => e.Description));
            return Result<string>.Failure($"Error updating user: {errors}");
        }

        return Result<string>.Success("User updated successfully");
    }
}