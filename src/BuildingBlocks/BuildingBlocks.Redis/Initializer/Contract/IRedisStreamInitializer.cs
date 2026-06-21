namespace BuildingBlocks.Shared.Events.Infrastructure.Initializer.Contract;

public interface IRedisStreamInitializer
{
    Task EnsureGroupExists(string groupName, string streamKey);
}