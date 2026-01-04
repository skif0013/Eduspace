using QuizService.Application.Contracts;
using QuizService.Application.DTOs;
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


    public async Task<Quiz> AddQuiz(CretingQuizDTO response, Guid userId)
    {
        // логика проверки уже существующего квиза а

        
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
        
        var save = _quizRepository.SaveQuizAsync(quiz);
        
        // change
        return save.Result;
    }

    public async Task<Quiz> FindByuIdAsync(Guid userId, Guid quizId)
    {
        var findQuiz = await _quizRepository.GetQuizAsync(quizId);
        
        
        return findQuiz;
    }

    public async Task<List<Quiz>> GetAllQuizesAsync(Guid userId)
    {
        var allQuizess = await _quizRepository.GetAllQuizzesAsync();
        
        
        //change
        return new List<Quiz>();
    }

    public async Task<Quiz> GetQuizAsync(Guid quizId)
    {
        var find = await _quizRepository.FindByIdAsync(quizId);
        
        // exeption
        
        
        return find;
        
    }

    // изменить на реквест дто
    public async Task<Quiz> UpdateQuizAsync(Guid quizId, CretingQuizDTO response)
    {
        
        
        
        var update = _quizRepository.UpdateQuizAsync(quizId);
        
        
        
        var updateQuiz = new Quiz()
        {
            QuizId = quizId,
            Category = response.Category,
            Name = response.QuizName,
            
        }
    }
}