namespace BuildingBlock.UserContextMiddleware.Models;

public class UserContext
{
    public Guid UserId { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
}