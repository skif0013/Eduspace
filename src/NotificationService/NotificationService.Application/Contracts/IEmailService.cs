using CourseService.Application.Courses.DTO;
using NotificationService.Application.DTOs;

namespace NotificationService.Application.Contracts;

public interface IEmailService
{
    public Task SendEmailAsync(EmailSendDTO dto);
    
    public Task SendVerifyEmailAsync(EmailVerifyDTO dto);
    public Task SendResetPasswordEmailAsync(ResetPasswordDTO dto);
    public Task SendFinishCoursMailAsync(CourseFinishDTO dto);
}