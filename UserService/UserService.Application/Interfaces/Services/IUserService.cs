using UserService.Application.DTO;
using UserService.Domain.Models;
using UserService.Domain.Results;

namespace UserService.Application.Interfaces.Services;

public interface IUserService
{
    Task<Result<string>> CreateUserAsync(CreateUserDTO request);
    Task<Result<User>> UpdateUserAsync(UpdateUserDTO request, Guid userId);
}