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
    private readonly IAttemptService _attemptService;

    public QuizController(IQuizService quizService, IAttemptService attemptService)
    {
        _quizService = quizService;
        _attemptService = attemptService;
    } 

    
    [HttpPost]
    public async Task<ActionResult<QuizResponseDTO>> Create([FromBody] CreatingQuizRequestDTO request)
    {
        var userId = Guid.NewGuid();
        
        var result = await _quizService.CreateQuizAsync(request, userId);
        
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<QuizResponseDTO>> GetById([FromRoute] Guid id)
    {
        await _quizService.GetQuizByIdAsync(id);
        
        return Ok();
    }

    
    [HttpGet]
    public async Task<ActionResult<IEnumerable<QuizResponseDTO>>> GetAll()
    {
        var result = await _quizService.GetAllQuizzesAsync();
        return Ok(result);
    }
    
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update([FromRoute] Guid id, [FromBody] QuizUpdateRequestDTO request)
    {
        await _quizService.UpdateQuizAsync(id, request);
        return NoContent();
    }
    
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete([FromRoute] Guid id)
    {
        await _quizService.DeleteQuizAsync(id);
        return NoContent();
    }
    

    [HttpPost("{id:guid}/start")]
    public async Task<ActionResult<QuizStartResponseDTO>> StartQuiz([FromRoute] Guid id)
    {
        var userId = Guid.NewGuid(); 
        var result = await _attemptService.StartQuizAsync(id, userId);
        return Ok(result);
    }

    [HttpPost("attempts/{attemptId:guid}/submit")]
    public async Task<ActionResult<SubmitAnswerResponseDTO>> SubmitAnswer(
        [FromRoute] Guid attemptId, 
        [FromBody] SubmitAnswerRequestDTO request)
    {
        var result = await _attemptService.SubmitAnswerAsync(attemptId, request);
        return Ok(result);
    }

    [HttpPatch("{id:guid}/publish")]
    public async Task<ActionResult<QuizResponseDTO>> PublishQuiz([FromRoute] Guid id)
    {
        var result = await _quizService.PublishQuizAsync(id);
        
        return Ok(result);
    }
    
    [HttpPost("{id:guid}/finish")]
    public async Task<ActionResult<FinishQuizResponseDTO>> FinishQuiz([FromRoute] Guid id)
    {
        var result = await _quizService.FinishQuizAsync(id);
        
        return Ok(result);
    }
}