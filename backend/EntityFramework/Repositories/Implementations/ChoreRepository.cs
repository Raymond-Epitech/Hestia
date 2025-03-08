using EntityFramework.Context;
using EntityFramework.Models;
using EntityFramework.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using Shared.Models.Input;
using Shared.Models.Output;

namespace EntityFramework.Repositories.Implementations
{
    public class ChoreRepository : IChoreRepository
    {
        private readonly HestiaContext _context;

        public ChoreRepository(HestiaContext context)
        {
            _context = context;
        }

        public async Task<List<ChoreOutput>> GetAllChoreOutputsAsync(Guid CollocationId)
        {
            var chores = await _context.Chore.Where(x => x.CollocationId == CollocationId).Select(x => new ChoreOutput
            {
                Id = x.Id,
                CreatedBy = x.CreatedBy,
                CreatedAt = x.CreatedAt,
                DueDate = x.DueDate,
                Title = x.Title,
                Description = x.Description,
                IsDone = x.IsDone
            }).ToListAsync();

            return chores;
        }

        public async Task<ChoreOutput?> GetChoreOutputByIdAsync(Guid id)
        {
            return await _context.Chore.Select(x => new ChoreOutput
            {
                Id = x.Id,
                CreatedBy = x.CreatedBy,
                CreatedAt = x.CreatedAt,
                DueDate = x.DueDate,
                Title = x.Title,
                Description = x.Description,
                IsDone = x.IsDone
            }).FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<List<ChoreMessageOutput>> GetAllChoreMessageOutputByChoreIdAsync(Guid choreId)
        {
            return await _context.ChoreMessage.Where(x => x.ChoreId == choreId).Select(x => new ChoreMessageOutput
            {
                Id = x.Id,
                CreatedBy = x.CreatedBy,
                CreatedAt = x.CreatedAt,
                Content = x.Content
            }).ToListAsync();
        }

        public async Task AddChoreAsync(Chore chore)
        {
            await _context.Chore.AddAsync(chore);
        }

        public async Task AddChoreMessageAsync(ChoreMessage choreMessage)
        {
            await _context.ChoreMessage.AddAsync(choreMessage);
        }

        public async Task<Chore?> GetChoreByIdAsync(Guid id)
        {
            return await _context.Chore.FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task UpdateChoreAsync(Chore chore)
        {
            _context.Chore.Update(chore);
        }

        public async Task<List<ChoreMessage>?> GetChoreMessageFromChoreId(Guid choreId)
        {
            return await _context.Chore.AnyAsync(x => x.Id == choreId)
            ? await _context.ChoreMessage.Where(x => x.ChoreId == choreId).ToListAsync()
            : null;
        }

        public async Task DeleteRangeChoreMessageFromChoreId(List<ChoreMessage> choreMessages)
        {
            _context.ChoreMessage.RemoveRange(choreMessages);
        }

        public async Task<List<UserOutput>> GetEnrolledUserOutputFromChoreIdAsync(Guid choreId)
        {
            var enroll = _context.ChoreEnrollments.Where(x => x.ChoreId == choreId);
            var users = await _context.User.Where(x => enroll.Select(x => x.UserId).Contains(x.Id)).Select(x => new UserOutput
            {
                Id = x.Id,
                Username = x.Username,
                Email = x.Email,
            }).ToListAsync();
            return users;
        }

        public async Task<List<ChoreOutput>> GetEnrolledChoreOutputFromUserIdAsync(Guid userId)
        {
            var enroll = _context.ChoreEnrollments.Where(x => x.UserId == userId);
            var chores = await _context.Chore.Where(x => enroll.Select(x => x.ChoreId).Contains(x.Id)).Select(x => new ChoreOutput
            {
                Id = x.Id,
                CreatedBy = x.CreatedBy,
                CreatedAt = x.CreatedAt,
                DueDate = x.DueDate,
                Title = x.Title,
                Description = x.Description,
                IsDone = x.IsDone
            }).ToListAsync();
            return chores;
        }

        public async Task AddChoreEnrollmentAsync(ChoreEnrollment choreEnrollment)
        {
            await _context.ChoreEnrollments.AddAsync(choreEnrollment);
        }

        public async Task<ChoreEnrollment?> GetChoreEnrollmentByUserIdAndChoreIdAsync(Guid userId, Guid choreId)
        {
            return await _context.ChoreEnrollments.Where(x => x.UserId == userId && x.ChoreId == choreId).FirstOrDefaultAsync();
        }

        public async Task RemoveChoreEnrollmentAsync(ChoreEnrollment choreEnrollment)
        {
            _context.ChoreEnrollments.Remove(choreEnrollment);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
