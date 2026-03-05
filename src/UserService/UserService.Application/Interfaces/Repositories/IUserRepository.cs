using UserService.Domain.Entities;

namespace UserService.Application.Interfaces.Repositories;

public interface IUserRepository
{
    Task<bool>  CreateUserAsync(User user);
    Task<bool> UpdateUserAsync(User user);
    Task<User> GetUserByIdAsync(Guid userId);
}