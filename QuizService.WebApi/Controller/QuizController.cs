using Microsoft.AspNetCore.Mvc;
using QuizService.Application.Contracts;
using QuizService.Application.DTOs;
using QuizService.Application.DTOs.QuizDTOs.ResponeDTO;

namespace QuizService.WebApi.Controller;

[ApiController]
[Route("api/[controller]")]
public class QuizController : ControllerBase
{
    private readonly IQuizService _quizService;
    private readonly ITokenService _tokenService;

    public QuizController(IQuizService quizService, ITokenService tokenService)
    {
        _tokenService = tokenService;
        _quizService = quizService;
    }
    
    [HttpPost("Create")]
    public async Task<ActionResult<QuizResponseDTO>> Create([FromBody] CreatingQuizRequestDTO request, [FromRoute] string token )
    {
        //   var userId = _tokenService.GetUserIdFromToken(token);
        
        var result = await _quizService.CreateQuizAsync(request);
        return CreatedAtAction(nameof(GetById), new { id = result.QuizId }, result);
    }

    
    [HttpGet("GetById")]
    public async Task<ActionResult<QuizResponseDTO>> GetById([FromRoute] Guid quizId, Guid userId)
    {
        var get = await _quizService.GetQuizByIdAsync(quizId,userId);
        
        return Ok(get);
    }

    
    [HttpGet("getAll")]
    public async Task<ActionResult<IReadOnlyCollection<QuizResponseDTO>>> GetAll()
    {
        var result = await _quizService.GetAllQuizzesAsync();
        return Ok(result);
    }
    
    
    [HttpPut("Update")]
    public async Task<IActionResult> Update([FromRoute] Guid quizId,[FromBody] QuizUpdateRequestDTO request)
    {
        await _quizService.UpdateQuizAsync(quizId, request);
        
        return NoContent();
    }
    
    [HttpDelete("Delete")]
    public async Task<IActionResult> Delete([FromRoute] Guid quizId)
    {
        await _quizService.DeleteQuizAsync(quizId);
        
        return NoContent();
    }
}