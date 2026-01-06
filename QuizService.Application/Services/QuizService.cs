using QuizService.Application.Contracts;
using QuizService.Application.DTOs;
using QuizService.Application.DTOs.QuizDTOs.ResponeDTO;
using QuizService.Application.Repositories;
using QuizService.Domain.Models;

namespace QuizService.Application.Services;

public class QuizService : IQuizService
{
    private readonly IQuizRepository _quizRepository;

    public QuizService(IQuizRepository quizRepository)
    {
        _quizRepository = quizRepository;
    }


    public async Task<QuizResponseDTO> CreateQuizAsync(CretingQuizRequestDTO response, Guid userId)
    {
        
        var quiz = new Quiz()
        {
            QuizId = Guid.NewGuid(),
            CreatorId = userId,
            Description = response.Text,
            IsPublished = true,
            IsActive = true,
            ModifiedOn = DateTime.Now,
            Name = response.QuizName,
            CreatedOn = DateTime.Now,
        };

        var add = await _quizRepository.AddQuizAsync(quiz);

        return add;
    }

    public async Task<Quiz> FindByIdAsync(Guid userId, Guid quizId)
    {
        var findQuiz = await _quizRepository.FindByIdAsync(quizId);
        
        
        return findQuiz;
    }
  
    
    //for developming
    public async Task<List<Quiz>> GetQuizzes()
    {
        var getAll = await _quizRepository.GetAllQuizzesAsync();
        
        
        //change
        return new List<Quiz>();
    }
    
    public async Task<Quiz?> UpdateQuizAsync(Guid quizId, QuizUpdateRequestDTO request, Guid userId)
    {
        
        var quiz = await _quizRepository.FindByIdAsync(quizId);

        
        
        var creatorId = quiz?.CreatorId;
        
        if (quiz != null)
        {
            quiz.Name = request.Title;
            quiz.Description = request.Description;
            quiz.IsActive = request.IsActive;
            quiz.IsPublished = request.IsPublished;
            quiz.ModifiedOn = DateTime.Now;

            var updatedQuiz = await _quizRepository.UpdateQuizAsync(quizId);
            return updatedQuiz;
        }
        
        // should be  request dto

        return quiz;
    }
    
    
    public async Task<Quiz> DeleteQuizAsync(Guid quizId, Guid userId)
    {
        var deletedQuiz = await _quizRepository.DeleteQuizAsync(quizId);
        return deletedQuiz;
    }
    
}