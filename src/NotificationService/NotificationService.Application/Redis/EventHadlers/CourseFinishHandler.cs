using BuildingBlocks.Redis.Events.Handler;
using BuildingBlocks.Shared.Events.Messaging.Contracts.Course;
using CourseService.Application.Courses.DTO;
using Microsoft.Extensions.DependencyInjection;
using NotificationService.Application.Contracts;

namespace NotificationService.Application.Redis.EventHadnlers;

public class CourseFinishHandler : ScopedMessageHandler<CourseFinishEvent>
{
    public CourseFinishHandler(IServiceScopeFactory scopeFactory) : base(scopeFactory)
    {
    }

    public override string Channel => "course:finish";
    protected override  async Task HandleMessageAsync(CourseFinishEvent @event, IServiceProvider serviceProvider)
    {
        var emailService =   serviceProvider.GetRequiredService<IEmailService>();
        
        var emailDto  = new CourseFinishDTO(@event.UserEmail, @event.UserName, @event.CourseId, @event.CourseTitle, @event.FinishDate, @event.FinishScore);
        
        
        await emailService.SendFinishCoursMailAsync(emailDto);
    }
}