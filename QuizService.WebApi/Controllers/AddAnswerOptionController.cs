using Microsoft.AspNetCore.Mvc;
using QuizService.Application.Contracts.AnswerOptionContracts;
using QuizService.Application.DTOs.AnswerOptionDTO.UpdateAnswerOption;
using QuizService.Application.DTOs.QuestionOptionDTO;

namespace QuizService.WebApi.Controller;

[ApiController]
[Route("api/[controller]")]
public class AddAnswerOptionController : ControllerBase
{
    private readonly IAnswerOption _answerOption;
    
    public AddAnswerOptionController(IAnswerOption answerOption)
    {
        _answerOption = answerOption;
    }

    [HttpPost]
    public async Task<IActionResult> AddAnswerOptionAsync([FromBody] AnswerOptionDTO dto,[FromRoute]  Guid questionId )
    {
        var result = await _answerOption.AddAnswerOptionAsync(dto, questionId);
        
        return new OkObjectResult(result);
    }

    [HttpGet]
    public async Task<IActionResult> GetAnswerOptionByIdAsync([FromRoute] Guid questionId)
    {
        var result = await _answerOption.GetAnswerOptionsByQuestionIdAsync(questionId);
        
        return new OkObjectResult(result);
    }

    [HttpDelete("{answerOptionId}/{questionId}")]
    public async Task<IActionResult> DeleteAnswerOptionAsync([FromRoute] Guid answerOptionId, Guid questionId)
    {
        var delete = await _answerOption.DeleteAnswerOptionAsync(answerOptionId, questionId);
        
        return NoContent();
    }

    [HttpPut]
    public async Task<IActionResult> UpdateAnswerOptionAsync([FromBody] UpdateAnswerOptionDTO dto, Guid questionId)
    {
         await _answerOption.UpdateAnswerOptionAsync(dto, questionId);
         
         return new OkObjectResult(true);
    }
}