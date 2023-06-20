using System.Text.Json;
using CommandAPI.Models;
using StackExchange.Redis;

namespace CommandAPI.Data
{
    public class RedisCommandRepo : ICommandRepo
    {
        private readonly IConnectionMultiplexer _redis;

        public RedisCommandRepo(IConnectionMultiplexer redis)
        {
            _redis = redis;
        }

        public async Task CreateCommandAsync(Command cmd)
        {
            if (cmd == null)
            {
                throw new ArgumentNullException(nameof(cmd));
            }

            var db = _redis.GetDatabase();
            var serialCommand = JsonSerializer.Serialize(cmd);
            await db.HashSetAsync($"commands", new HashEntry[] { new HashEntry(cmd.CommandId, serialCommand) });
        }

        public void DeleteCommand(Command cmd)
        {
            var db = _redis.GetDatabase();
            db.HashDelete("commands", cmd.CommandId);

        }

        public async Task<IEnumerable<Command?>?> GetAllCommandsAsync()
        {
            var db = _redis.GetDatabase();
            var completeSet = await db.HashGetAllAsync("commands");

            if (completeSet.Length > 0)
            {
                return Array.ConvertAll(completeSet, val => JsonSerializer.Deserialize<Command>(val.Value));
            }

            var empty = new List<Command>();

            return empty;
        }

        public async Task<Command?> GetCommandByIdAsync(string commandId)
        {
            var db = _redis.GetDatabase();
            var command = await db.HashGetAsync("commands", commandId);

            if (!string.IsNullOrWhiteSpace(command))
            {
                return JsonSerializer.Deserialize<Command>(command);
            }

            return null;
        }

        public async Task SaveChangesAsync()
        {
            Console.WriteLine("--> SaveChangesAsync in RedisCommandRepo called redundantly...");
            await Task.CompletedTask;
        }

        public async Task UpdateCommandAsync(Command cmd)
        {
            var db = _redis.GetDatabase();
            var serialCommand = JsonSerializer.Serialize(cmd);

            await db.HashSetAsync($"commands", new HashEntry[] { new HashEntry(cmd.CommandId, serialCommand) });
        }
    }
}