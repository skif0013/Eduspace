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


    [HttpGet("GetQuestionById")]
    public async Task<IActionResult> GetQuestionById(Guid questionId)
    {
        var question = await _questionService.GetQuestionByIdAsync(questionId);
        
        return new JsonResult(question);
    }

    [HttpPost("AddQuestionToQuiz")]
    public async Task<IActionResult> AddQuestionToQuiz(CreateQuestionRequestDTO requestDTO)
    {
        var add = await _questionService.AddQuestionToQuizAsync(requestDTO);
            
        return new JsonResult(add);
    }


    [HttpGet("GetAllQuestions")]
    public async Task<IActionResult> GetAllQuestions()
    {
        var getAll = await  _questionService.GetAllQuestionsAsync();
        
        return new JsonResult(getAll);
    }

    [HttpDelete("DeleteQuestionFromQuiz")]
    public async Task<IActionResult> DeleteQuestionFromQuiz(Guid questionId)
    {
        var delete = _questionService.DeleteQuestionFromQuizAsync(questionId);

        return NoContent();
    }

    [HttpPut("UpdateQuestionToQuiz")]
    public async Task<IActionResult> UpdateQuestionToQuiz(CreateQuestionRequestDTO requestDto, Guid questionId)
    {
        var update = await _questionService.UpdateQuestionToQuizAsync(requestDto, questionId);
        
        return new JsonResult(update);
    }
}