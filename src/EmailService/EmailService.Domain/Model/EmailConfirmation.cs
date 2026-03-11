namespace EmailService.Domain.Model;

public class EmailConfirmation
{
    public string Email { get; set; } = string.Empty;
    
    public string ConfirmationCode { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    public bool IsExpired => DateTime.UtcNow > CreatedAt.AddMinutes(5);
    
    public bool IsUsed { get; set; }

    public bool Verify(string inputCode)
    {
        if (IsUsed) throw new Exception("Code already used");
        if (IsExpired) throw new Exception("Code expired");

        if (ConfirmationCode == inputCode)
        {
            IsUsed = true;
            return true;
        }

        return false;
    }
}