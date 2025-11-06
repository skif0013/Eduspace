using AuthService.Application.DTOs;
using AuthService.Application.Interfaces;
using AuthService.Domain.Results;
using Microsoft.AspNetCore.Mvc;

namespace AuthService.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly IUserService _userService;

    public UsersController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpPost("register")]
    public async Task<Result<string>> Register([FromBody] CreateUserDto createUserDto)
    {
        var result = await _userService.RegisterAsync(createUserDto);
        return result;
    }

    [HttpPost("login")]
    public async Task<Result<AuthResponse>> LoginAsync(AuthRequest request)
    {
        var result = await _userService.AuthenticateAsync(request);
        return result;
    }

    [HttpPost("ForgotPassword")]
    public async Task<Result<string>> ForgotPasswordAsync(ForgotPasswordRequest request)
    {
        var result = await _userService.ForgotPasswordAsync(request);
        return result;
    }

    [HttpPost("ResetPassword")]
    public async Task<Result<string>> ResetPasswordAsync(ResetPasswordDto request)
    {
        var result = await _userService.ResetPasswordAsync(request);
        return result;
    }

    [HttpPost("ConfirmEmail")]
    public async Task<Result<string>> ConfimEmailAsync(string email, [FromHeader] string token)
    {
        var result = await _userService.ConfirmEmailAsync(email, token);
        return result;
    }
}

