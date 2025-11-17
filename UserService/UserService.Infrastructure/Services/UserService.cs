using UserService.Application.DTO;
using UserService.Application.Interfaces.Repositories;
using UserService.Application.Interfaces.Services;
using UserService.Domain.Models;
using UserService.Domain.Results;

namespace UserService.Infrastructure.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;
    
    public UserService(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }
    
    public async Task<Result<User>> GetUserByIdAsync(Guid userId)
    {
        var user = await _userRepository.GetUserByIdAsync(userId);
        if (user == null)
        {
            return Result<User>.Failure("User not found");
        }
        
        return Result<User>.Success(user);
    }
    
    public async Task<Result<string>> CreateUserAsync(CreateUserDTO request)
    {
        var existingUser = await _userRepository.GetUserByIdAsync(request.UserId);
        if (existingUser != null)
        {
            return Result<string>.Failure("User already exists");
        }
        
        var user = new User
        {
            UserId = request.UserId,
            UserName = request.UserName,
            Email = request.Email
        };
        
        await _userRepository.CreateUserAsync(user);
        
        return Result<string>.Success("User created successfully");
    }
    
    public async Task<Result<User>> UpdateUserAsync(UpdateUserDTO request, Guid userId)
    {
        var user = await _userRepository.GetUserByIdAsync(userId);
        if (user == null)
        {
            return Result<User>.Failure("User not found");
        }
        
        user.UserName = request.UserName;
        user.Email = request.Email;
        user.AvatarURL = request.AvatarURL;
        
        await _userRepository.UpdateUserAsync(user);
        
        return Result<User>.Success(user);
    }
}