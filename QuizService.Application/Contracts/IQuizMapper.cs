using QuizService.Application.DTOs;
using QuizService.Application.DTOs.QuizDTOs.ResponeDTO;
using QuizService.Application.DTOs.ResponseDTOs;
using QuizService.Domain.Models;

namespace QuizService.Application.Contracts;

public class IQuizMapper
{
    Quiz MapToDomain(CreatingQuizRequestDTO request, Guid userId);
    
    void MapToDomain(QuizUpdateRequestDTO request, Quiz quiz);

    public QuizResponseDTO MapToResponseDTO(Quiz quiz);
}