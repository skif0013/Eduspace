using System.Text.Json;
using IdentityService.Application.DTOs;
using IdentityService.Application.Interfaces;
using IdentityService.Application.Interfaces.Repositories;
using IdentityService.Application.Interfaces.Services;
using IdentityService.Domain.Entities;
using IdentityService.Domain.Results;
using IdentityService.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Shared.Messages;
using BuildingBlocks.Redis.Events;

namespace IdentityService.Infrastructure.Services;

public class UserService : IUserService
{
    
    private readonly UserManager<User> _userManager;
    private readonly RoleManager<RoleIdentity> _roleManager;
    private readonly ITokenService _tokenService;
    private readonly IUnitOfWork _unitOfWork;

    public UserService(
        UserManager<User> userManager,
        RoleManager<RoleIdentity> roleManager,
        ITokenService tokenService,
        IUnitOfWork unitOfWork
    )
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _tokenService = tokenService;
        _unitOfWork = unitOfWork;
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
        Console.WriteLine($"Email confirmation token for {user.Email}: {token}");
        
        var confirmEmailEvent = new EmailVerifyEvent()
        {
            To = user.Email,
            UserName = user.UserName,
            VerificationLink = "",
            Code = token
        };
        
        var outboxMessage = new OutboxMessage()
        {
            Id = Guid.NewGuid(),
            Type = "identity.user.created",
            Content = JsonSerializer.Serialize(confirmEmailEvent),
            OccurredOnUtc = DateTime.UtcNow
        };

        await _unitOfWork.OutboxRepository.AddAsync(outboxMessage);
        await _unitOfWork.Commit();
        
        return Result<string>.Success("user created successfully");
    }

    public async Task<Result<TokenResponseDto>> AuthenticateAsync(AuthRequest request)
    {
        var user = await _userManager.FindByEmailAsync(request.Email);
        if (user == null)
        {
            return Result<TokenResponseDto>.Failure("Invalid Email");
        }

        var isEmailConfirmed = await _userManager.IsEmailConfirmedAsync(user);
        if (!isEmailConfirmed)
        {
            return Result<TokenResponseDto>.Failure("Email not confirmed");
        }

        var isPasswordCorrect = await _userManager.CheckPasswordAsync(user, request.Password);
        if (!isPasswordCorrect)
        {
            return Result<TokenResponseDto>.Failure("Invalid Password");
        }
        
        var userDto = new UserDto
        {
            Id = user.Id,
            UserName = user.UserName,
            Email = user.Email
        };
        
        var tokens = await _tokenService.GetTokens(userDto);
        
        return Result<TokenResponseDto>.Success(new TokenResponseDto
        {
            AccessToken = tokens.AccessToken,
            RefreshToken = tokens.RefreshToken
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
        
        var confirmEmailEvent = new UserResetPasswordEvent()
        {
            UserEmail = request.Email,
            Token = token
        };
        
        var outboxMessage = new OutboxMessage()
        {
            Id = Guid.NewGuid(),
            Type = "identity.user.forgot_password",
            Content = JsonSerializer.Serialize(confirmEmailEvent),
            OccurredOnUtc = DateTime.UtcNow
        };

        await _unitOfWork.OutboxRepository.AddAsync(outboxMessage);
        await _unitOfWork.Commit();
        
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

    public async Task<Result<string>> ConfirmEmailAsync(ConfirmEmailRequest request)
    {
        var user = await _userManager.FindByEmailAsync(request.Email);
        if (user == null)
        {
            return Result<string>.Failure("Invalid email address");
        }
        await _userManager.ConfirmEmailAsync(user, request.Token);
        
        var roleExists = _roleManager.FindByNameAsync("User");
        if (!roleExists.IsCompletedSuccessfully)
        {
            Console.WriteLine("Role doesn't exist");
        }

        await _userManager.AddToRoleAsync(user, roleExists.Result.Name);
        
        return Result<string>.Success("Email confirmed");
    }
    
    public async Task<Result<string>> UpdateUserAsync(UpdateUserDTO userDto) // TODO переделать на IUserContext + добавить смену пароля + добавить в будущем таблицу с юзер инфо где будет урла на его автарку
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