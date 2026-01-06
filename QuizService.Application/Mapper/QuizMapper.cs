using QuizService.Application.Contracts;
using QuizService.Application.DTOs;
using QuizService.Application.DTOs.QuizDTOs.ResponeDTO;
using QuizService.Domain.Models;

namespace QuizService.Application.Services;

public class QuizMapper : IQuizMapper
{
    public Quiz MapToQuizDomainModel(CreatingQuizRequestDTO dto, Guid userId)
    {
        return new Quiz
        {
            QuizId = Guid.NewGuid(),
            CreatorId = userId,
            Name = dto.QuizName,
            Description = dto.Description,
            Category = dto.Category,
            IsActive = true,
            IsPublished = false,
            CreatedOn = DateTime.UtcNow,
            ModifiedOn = DateTime.UtcNow
        };
    }

    public void MapToDomain(QuizUpdateRequestDTO request, Quiz quiz)
    {
        quiz.Name = request.Title;
        quiz.Description = request.Description;
        quiz.Category = request.Category;
        quiz.IsActive = request.IsActive;
        quiz.IsPublished = request.IsPublished;
        quiz.ModifiedOn = DateTime.UtcNow;
    }
    
    public QuizResponseDTO MapToResponse(Quiz quiz)
    {
        return new QuizResponseDTO
        {
            QuizId = quiz.QuizId,
            Name = quiz.Name,
            Description = quiz.Description,
            Category = quiz.Category,
        };
    }
}