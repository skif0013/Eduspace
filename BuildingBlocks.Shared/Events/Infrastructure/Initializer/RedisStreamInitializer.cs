using BuildingBlocks.Shared.Events.Infrastructure.Initializer.Contract;
using StackExchange.Redis;

namespace BuildingBlocks.Shared.Events.Infrastructure.Initializer;
//ВЫНЕСТИ ЕГО ИЗ ШЕРЕД СЛОЯ 
public class RedisStreamInitializer : IRedisStreamInitializer
{
    private readonly IConnectionMultiplexer _multiplexer;

    
    public RedisStreamInitializer(IConnectionMultiplexer multiplexer)
    {
        _multiplexer = multiplexer;
    }

    public async Task EnsureGroupExists(string groupName, string streamKey)
    {
        var database = _multiplexer.GetDatabase();
        
        var groupInfo  =  await  database.StreamGroupInfoAsync(streamKey);
        
        if(groupInfo.All(g => g.Name != groupName))
        {
            await database.StreamCreateConsumerGroupAsync(streamKey, groupName, "$");
        }
        
        await CreateGroupASync(groupName, streamKey);
    }

    public Task CreateGroupASync(string groupName, string streamKey)
    {
        var database = _multiplexer.GetDatabase();
        return database.StreamCreateConsumerGroupAsync(streamKey, groupName, "$");
    }
}