using Microsoft.EntityFrameworkCore;
using QuizService.Application.Contracts.QuestionsContract;
using QuizService.Domain.Models;
using QuizService.Infrastructure.Data;

namespace QuizService.Infrastructure.Repositories;

public class QuestionRepository : IQuestionRepository
{
    private readonly ApplicationDbContext _context;
    
    public QuestionRepository(ApplicationDbContext context)
    {
        _context = context;
    }
    
    
    public async Task<IReadOnlyCollection<Question>> GetAllQuestionsAsync() =>
        await _context.Questions.ToListAsync();
    
    public  Task AddQuestion(Question question)
    {
        _context.Questions.Add(question);
        
        return Task.CompletedTask;
    }


    public async Task<Question?> FindByIdAsync(Guid questionId)
    {
        return await _context.Questions.Include(q => q.AnswerOptions).FirstOrDefaultAsync(q => q.Id == questionId);
    }
        

    public Task RemoveAsync(Question question)
    {
        _context.Remove(question);
        
        return Task.CompletedTask;
    }
}