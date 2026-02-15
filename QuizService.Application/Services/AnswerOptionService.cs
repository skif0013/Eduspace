using QuizService.Application.Contracts.AnswerOptionContracts;
using QuizService.Application.Contracts.QuestionsContract;
using QuizService.Application.DTOs.QuestionOptionDTO;
using QuizService.Application.Repositories;
using QuizService.Domain.Models;
using System.Linq;
using QuizService.Application.DTOs.AnswerOptionDTO.UpdateAnswerOption;
using QuizService.Application.DTOs.QuestionOptionDTO.ResponseDTO;

namespace QuizService.Application.Services;

public class AnswerOptionService : IAnswerOption
{
    private readonly IQuestionRepository _questionRepository;
    private readonly IUnitOfWork _unitOfWork;
    
    public AnswerOptionService(IQuestionRepository questionRepository, IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
        _questionRepository = questionRepository;
    }

    public async Task<AnswerOptionResponseDTO> AddAnswerOptionAsync(AnswerOptionDTO dto, Guid questionId)
    {
        var question = await _questionRepository.FindByIdAsync(questionId);

        var option = new AnswerOption()
        {
            Id = Guid.NewGuid(),
            Text = dto.Text,
            IsCorrectAnswer = dto.IsCorrect,
            QuestionId = questionId,
            Score = dto.Score,
            Order = dto.Order,
            ModifiedOn = DateTime.UtcNow,
            CreatedOn = DateTime.UtcNow
        };
        
        if (question == null) throw new Exception("Question not found");

        question.AddAnswerOption(option);

        await _unitOfWork.SaveChangesAsync();
        
        return new AnswerOptionResponseDTO()
        {
            Id = option.Id,
            Text = option.Text,
            IsCorrect = option.IsCorrectAnswer,
            Score = option.Score,
            Order = option.Order
        };
    }

    public async Task UpdateAnswerOptionAsync(UpdateAnswerOptionDTO dto, Guid questionId)
    {
        var question = await _questionRepository.FindByIdAsync(questionId);
        
        var option = question.AnswerOptions.FirstOrDefault(o => o.Id == dto.Id);
        
        if (option == null)
        {
            throw new Exception("Answer option not found");
        }
        
        option.Text = dto.Text;
        option.IsCorrectAnswer = dto.IsCorrect;
        option.Score = dto.Score;
        option.Order = dto.Order;
        
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task<bool> DeleteAnswerOptionAsync(Guid answerOptionId, Guid questionId)
    {
        var question = await _questionRepository.FindByIdAsync(questionId);
        
        var option = question.AnswerOptions.FirstOrDefault(o => o.Id == answerOptionId);
        
        if (option == null)
        {
            throw new Exception("Answer option not found");
        }
        
        question.RemoveAnswerOption(option);
        
        await _unitOfWork.SaveChangesAsync();
        
        return true;
    }
    
    public async Task<IReadOnlyCollection<AnswerOptionResponseDTO>> GetAnswerOptionsByQuestionIdAsync(Guid questionId)
    {
        var question = await _questionRepository.FindByIdAsync(questionId);
        
        if (question == null)
        {
            throw new Exception("Question not found");
        }
        
        var options = question.AnswerOptions.Select(o => new AnswerOptionResponseDTO() 
        {
            Id = o.Id,
            Text = o.Text,
            IsCorrect = o.IsCorrectAnswer,
            Score = o.Score,
            Order = o.Order
        }).ToList();
        
        return options;
    }
}