namespace BuildingBlocks.Redis.Events;

public class UserResetPasswordEvent
{
    public string To { get; set; }
    public string Token { get; set; }
}