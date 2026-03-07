using QuizService.Application.DTOs;

namespace QuizService.Application.Contracts;

public interface ITokenService
{
   UserContextDTO GetUserFromToken(string token);
   
   Guid GetUserIdFromToken(string token);
   
}