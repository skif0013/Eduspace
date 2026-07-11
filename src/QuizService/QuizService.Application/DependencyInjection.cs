using Microsoft.Extensions.DependencyInjection;
using QuizService.Application.Contracts;
using QuizService.Application.Contracts.IQuizAttempt;
using QuizService.Application.Contracts.QuestionsContract;
using QuizService.Application.Services;

namespace QuizService.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddQuizApplicationServices(this IServiceCollection services)
    {
        services.AddScoped<IQuizService, Services.QuizService>();
        services.AddScoped<ITokenService, TokenService>();
        services.AddScoped<IQuestionService, QuestionService>();
        services.AddScoped<IQuestionScoringService, QuestionScoringService>();
        services.AddScoped<IAttemptService, AttemptService>();

        services.AddScoped<IQuizMapper, QuizMapper>();
        services.AddScoped<IQuestionMapper, QuestionMapper>();

        return services;
    }
}
