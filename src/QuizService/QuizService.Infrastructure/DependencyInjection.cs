using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using QuizService.Application.Contracts;
using QuizService.Application.Contracts.IQuizAttempt;
using QuizService.Application.Contracts.QuestionsContract;
using QuizService.Application.Repositories;
using QuizService.Infrastructure.Data;
using QuizService.Infrastructure.Repositories;
using QuizService.Infrastructure.Services;

namespace QuizService.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddQuizInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration,
        IHostEnvironment environment)
    {
        if (!environment.IsEnvironment("Testing"))
        {
            services.AddDatabase(configuration);
        }

        services.AddRepositories();
        services.AddScoped<IQuizIntegrationEventService, NoOpQuizIntegrationEventService>();

        return services;
    }

    private static IServiceCollection AddDatabase(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseNpgsql(connectionString));

        return services;
    }

    private static IServiceCollection AddRepositories(this IServiceCollection services)
    {
        services.AddScoped<IQuizRepository, QuizRepository>();
        services.AddScoped<IQuestionRepository, QuestionRepository>();
        services.AddScoped<IAttemptRepository, AttemptRepository>();
        services.AddScoped<IUnitOfWork, Persistence.UnitOfWork.UnitOfWork>();

        return services;
    }
}
