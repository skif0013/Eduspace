using Microsoft.AspNetCore.Mvc;
using NotificationService.Application.Contracts;
using NotificationService.Application.DTOs;
using NotificationService.Infrastructure.Service;

namespace NotificationService.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EmailController : ControllerBase
{
    private readonly IEmailService _emailService;
    
    public EmailController(IEmailService emailService)
    {
        _emailService = emailService;
    }
    
    [HttpPost("send")]
    public async Task<IActionResult> SendEmailAsync(EmailSendDTO dto)
    {
        await  _emailService.SendEmailAsync(dto);
        
        return Ok("Email sent successfully");
    }
    
    [HttpPost("send-verify")]
    public async Task<IActionResult> SendVerifyEmailAsync(EmailVerifyDTO dto)
    {
        await  _emailService.SendVerifyEmailAsync(dto);
        
        return Ok("Verification email sent successfully");
    }
}