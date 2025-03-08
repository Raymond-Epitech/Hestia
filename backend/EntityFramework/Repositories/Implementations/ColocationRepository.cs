using EntityFramework.Context;
using EntityFramework.Models;
using EntityFramework.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using Shared.Models.Output;

namespace EntityFramework.Repositories.Implementations
{
    public class ColocationRepository : IColocationRepository
    {
        private readonly HestiaContext _context;

        public ColocationRepository(HestiaContext context)
        {
            _context = context;
        }

        public async Task<List<ColocationOutput>> GetAllColocationOutputAsync()
        {
            return await _context.Colocation.Select(x => new ColocationOutput
            {
                Id = x.Id,
                Name = x.Name,
                Address = x.Address,
                Colocataires = null
            }).ToListAsync();
        }

        public async Task<ColocationOutput?> GetColocationOutputFromIdAsync(Guid colocationId)
        {
            return await _context.Colocation.Where(x => x.Id == colocationId).Select(x => new ColocationOutput
            {
                Id = x.Id,
                Name = x.Name,
                Address = x.Address,
                Colocataires = x.Users.Select(u => u.Id).ToList()
            }).FirstOrDefaultAsync();
        }

        public async Task<Colocation?> GetColocationFromIdAsync(Guid colocationId)
        {
            return await _context.Colocation.FindAsync(colocationId);
        }

        public async Task AddColocationAsync(Colocation colocation)
        {
            await _context.Colocation.AddAsync(colocation);
        }

        public async Task UpdateColocationAsync(Colocation colocation)
        {
            _context.Colocation.Update(colocation);
        }

        public async Task DeleteColocationAsync(Colocation colocation)
        {
            _context.Colocation.Remove(colocation);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
