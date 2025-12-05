using AuthService.Application.DTOs;
using AuthService.Domain.Results;

namespace AuthService.Application.Interfaces;

public interface IUserService
{
    Task<Result<AuthResponse>> AuthenticateAsync(AuthRequest request);
    Task<Result<string>> RegisterAsync(CreateUserDto user);
    Task<Result<string>> ForgotPasswordAsync(ForgotPasswordRequest request);
    Task<Result<string>> ResetPasswordAsync(ResetPasswordDto request);
    Task<Result<string>> ConfirmEmailAsync(string email, string token);
    Task<Result<string>> UpdateUserAsync(UpdateUserDTO userDto);
}