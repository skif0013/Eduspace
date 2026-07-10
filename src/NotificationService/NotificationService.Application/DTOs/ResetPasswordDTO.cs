namespace NotificationService.Application.DTOs;

public record ResetPasswordDTO(
    string To,
    string Token
    );