using IdentityService.Application.DTOs;
using IdentityService.Domain.Results;

namespace IdentityService.Application.Interfaces;

public interface IUserService
{
    Task<Result<TokenResponseDto>> AuthenticateAsync(AuthRequest request);
    Task<Result<string>> RegisterAsync(CreateUserDto user);
    Task<Result<string>> ForgotPasswordAsync(ForgotPasswordRequest request);
    Task<Result<string>> ResetPasswordAsync(ResetPasswordDto request);
    Task<Result<string>> ConfirmEmailAsync(ConfirmEmailRequest request);
    Task<Result<string>> UpdateUserAsync(UpdateUserDTO userDto);
}