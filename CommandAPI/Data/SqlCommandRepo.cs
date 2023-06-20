using CommandAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace CommandAPI.Data
{
    public class SqlCommandRepo : ICommandRepo
    {
        private readonly AppDbContext _context;

        public SqlCommandRepo(AppDbContext context)
        {
            _context = context;
        }

        public async Task CreateCommandAsync(Command cmd)
        {
            if (cmd == null)
            {
                throw new ArgumentNullException(nameof(cmd));
            }

            await _context.AddAsync(cmd);
        }

        public void DeleteCommand(Command cmd)
        {
            if (cmd == null)
            {
                throw new ArgumentNullException(nameof(cmd));
            }

            _context.Commands.Remove(cmd);
        }

        public async Task<IEnumerable<Command?>?> GetAllCommandsAsync()
        {
            return await _context.Commands.ToListAsync();
        }

        public async Task<Command?> GetCommandByIdAsync(string commandId)
        {
            return await _context.Commands.FirstOrDefaultAsync(c => c.CommandId == commandId);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }

        public async Task UpdateCommandAsync(Command cmd)
        {
            Console.WriteLine("--> UpdateCommandAsync in SqlCommandRepo called redundantly...");

            await Task.CompletedTask;
        }
    }
}