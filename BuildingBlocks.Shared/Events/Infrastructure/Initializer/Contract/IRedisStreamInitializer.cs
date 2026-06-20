namespace BuildingBlocks.Shared.Events.Infrastructure.Initializer.Contract;

public interface IRedisStreamInitializer
{
    Task EnsureGroupExists(string groupName, string streamKey);
    
    Task CreateGroupASync(string groupName, string streamKey);
}