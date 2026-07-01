namespace BuildingBlocks.Redis.Events;

public class UserResetPasswordEvent
{
    public string UserEmail { get; set; }
    public string Token { get; set; }
}