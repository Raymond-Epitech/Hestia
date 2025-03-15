using EntityFramework.Context;
using EntityFramework.Models;
using EntityFramework.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
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
            return await _context.Reminders.Where(x => x.ColocationId == colocationId).Select(x => new ReminderOutput
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
            return await _context.Reminders.Select(x => new ReminderOutput
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
            return await _context.Reminders.FindAsync(id);
        }

        public async Task AddReminderAsync(Reminder reminder)
        {
            await _context.Reminders.AddAsync(reminder);
        }

        public async Task UpdateReminderAsync(Reminder reminder)
        {
            _context.Reminders.Update(reminder);
        }

        public async Task DeleteReminderAsync(Reminder reminder)
        {
            _context.Reminders.Remove(reminder);
        }

        public async Task<List<Reminder>> GetReminderFromListOfId(List<Guid> ids)
        {
            return await _context.Reminders.Where(x => ids.Contains(x.Id)).ToListAsync();
        }

        public async Task<IDbContextTransaction> BeginTransactionAsync()
        {
            return await _context.Database.BeginTransactionAsync();
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
