namespace BuildingBlocks.Redis.Events;

public class UserCreatedEvent
{
    public string To { get; set; }
    public string UserName { get; set; }
    public string VerificationLink { get; set; }
    public string Code { get; set; }
}