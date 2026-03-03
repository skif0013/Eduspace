using Microsoft.AspNetCore.Mvc;
using QuizService.Application.Contracts.QuestionsContract;
using QuizService.Application.DTOs.QuestionsDTOs.CreateRequestDTOs;

namespace QuizService.WebApi.Controller;

[ApiController]
[Route("api/[controller]")]
public class QuestionController : ControllerBase
{
    private readonly IQuestionService _questionService;
    
    public QuestionController(IQuestionService questionService)
    {
        _questionService = questionService;
    }

    
    [HttpGet("{id:guid}")] 
    public async Task<IActionResult> GetById(Guid id)
    {
        var question = await _questionService.GetQuestionByIdAsync(id);
        if (question == null) return NotFound(); 
        
        return Ok(question);
    }

    
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateQuestionRequestDTO requestDTO)
    {
        var result = await _questionService.CreateQuestionWithTitleAsync(requestDTO);
            
        return CreatedAtAction(nameof(GetById), new { id = result.QuestionId }, result);
    }

    
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var questions = await _questionService.GetAllQuestionsAsync();
        return Ok(questions);
    }

   
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        await _questionService.DeleteQuestionFromQuizAsync(id);
        return NoContent(); // 204 — Успешно, но возвращать нечего
    }

    
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] CreateQuestionRequestDTO requestDto)
    {
        var result = await _questionService.UpdateQuestionToQuizAsync(requestDto, id);
        return Ok(result);
    }

    
    [HttpPatch("{id:guid}/complete")]
    public async Task<IActionResult> Complete(Guid id)
    {
        var result = await _questionService.CompletedQuestionAsync(id);
        return Ok(result);
    }
}