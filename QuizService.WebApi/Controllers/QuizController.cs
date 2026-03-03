using Microsoft.AspNetCore.Mvc;
using QuizService.Application.Contracts;
using QuizService.Application.Contracts.IQuizAttempt;
using QuizService.Application.DTOs;
using QuizService.Application.DTOs.QuizDTOs;
using QuizService.Application.DTOs.QuizDTOs.ResponeDTO;

namespace QuizService.WebApi.Controller;

[ApiController]
[Route("api/[controller]")]
public class QuizController : ControllerBase
{
    private readonly IQuizService _quizService;
    private readonly ITokenService _tokenService;
    private readonly IAttemptService _attemptService;

    public QuizController(IQuizService quizService, ITokenService tokenService, IAttemptService attemptService)
    {
        _attemptService = attemptService;
        _tokenService = tokenService;
        _quizService = quizService;
    } 

    [HttpPost("Create")]
    public async Task<ActionResult<QuizResponseDTO>> Create([FromBody] CreatingQuizRequestDTO request)
    {
        // var userId = _tokenService.GetUserIdFromToken(token);

        var result = await _quizService.CreateQuizAsync(request);
        return CreatedAtAction(nameof(GetById), new { id = result.QuizId }, result);
    }


    [HttpPost("{id:guid}/Start")]
    public async Task<ActionResult<QuizStartResponseDTO>> StartQuiz([FromRoute] Guid id)
    {
        var userId = Guid.NewGuid(); 
        
        var result = await _attemptService.StartQuizAsync(id, userId);
        
        return Ok(result);
    }

    [HttpPost("{attemptId:guid}/SubmitAnswer")]
    public async Task<ActionResult<SubmitAnswerResponseDTO>> SubmitAnswer([FromRoute] Guid attemptId, SubmitAnswerRequestDTO request)
    {
        var result = await _attemptService.SubmitAnswerAsync(attemptId, request);
        
        return Ok(result);
    }


    [HttpGet("{id:guid}")]
    public async Task<ActionResult<QuizResponseDTO>> GetById([FromRoute] Guid quizId, Guid userId)
    {
        var get = await _quizService.GetQuizByIdAsync(quizId, userId);

        return Ok(get);
    }


    [HttpGet]
    public async Task<ActionResult<IReadOnlyCollection<QuizResponseDTO>>> GetAll()
    {
        var result = await _quizService.GetAllQuizzesAsync();
        return Ok(result);
    }


    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update([FromRoute] Guid quizId, [FromBody] QuizUpdateRequestDTO request)
    {
        await _quizService.UpdateQuizAsync(quizId, request);

        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete([FromRoute] Guid quizId)
    {
        await _quizService.DeleteQuizAsync(quizId);

        return NoContent();
    }
}