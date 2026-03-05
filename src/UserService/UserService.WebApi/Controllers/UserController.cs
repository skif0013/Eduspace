using Microsoft.AspNetCore.Mvc;
using UserService.Application.DTO;
using UserService.Application.Interfaces.Services;
using UserService.Domain.Entities;
using UserService.Domain.Results;

namespace UserService.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UserController : ControllerBase
{
    private readonly IUserService _userService;

    public UserController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpGet]
    [Route("{userId}")]
    public async Task<Result<User>> GetUserById(Guid userId)
    {
        var result = await _userService.GetUserByIdAsync(userId);
        return result;
    }
    
    [HttpPost]
    [Route("create")]
    public async Task<Result<string>> CreateUser(CreateUserDTO request)
    {
        var result = await _userService.CreateUserAsync(request);
        return result;
    }

    [HttpPut]
    [Route("update")]
    public async Task<Result<User>> UpdateUser(UpdateUserDTO request)
    {
        //var userIdString = User.FindFirst("userId")?.Value;
        //var userId = Guid.Parse("019ae437-9389-78e2-a48f-2309db82fa59");
        var userId = Guid.Parse(Request.Headers["X-User-Id"]);
        var result = await _userService.UpdateUserAsync(request, userId);
        return result;
    }
    
}