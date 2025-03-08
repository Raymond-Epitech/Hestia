using EntityFramework.Context;
using EntityFramework.Models;
using EntityFramework.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using Shared.Models.Output;

namespace EntityFramework.Repositories.Implementations
{
    public class ReminderRepository : IReminderRepository
    {
        private HestiaContext _context;

        public ReminderRepository(HestiaContext context)
        {
            _context = context;
        }

        public async Task<List<ReminderOutput>> GetAllReminderOutputsAsync(Guid colocationId)
        {
            return await _context.Reminder.Where(x => x.ColocationId == colocationId).Select(x => new ReminderOutput
            {
                Id = x.Id,
                Content = x.Content,
                Color = x.Color,
                CoordX = x.CoordX,
                CoordY = x.CoordY,
                CoordZ = x.CoordZ
            }).ToListAsync();
        }

        public async Task<ReminderOutput?> GetReminderOutputAsync(Guid id)
        {
            return await _context.Reminder.Select(x => new ReminderOutput
            {
                Id = x.Id,
                Content = x.Content,
                Color = x.Color,
                CreatedBy = x.CreatedBy,
                CreatedAt = x.CreatedAt,
                CoordX = x.CoordX,
                CoordY = x.CoordY,
                CoordZ = x.CoordZ
            }).FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<Reminder?> GetReminderAsync(Guid id)
        {
            return await _context.Reminder.FindAsync(id);
        }

        public async Task AddReminderAsync(Reminder reminder)
        {
            await _context.Reminder.AddAsync(reminder);
        }

        public async Task UpdateReminderAsync(Reminder reminder)
        {
            _context.Reminder.Update(reminder);
        }

        public async Task DeleteReminderAsync(Reminder reminder)
        {
            _context.Reminder.Remove(reminder);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
