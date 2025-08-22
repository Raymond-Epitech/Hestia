using Business.Interfaces;
using Business.Mappers;
using EntityFramework.Models;
using EntityFramework.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Shared.Models.Input;
using Shared.Models.Output;
using Shared.Models.Update;

namespace Business.Services;

public class PollService(ILogger<PollService> logger,
    IRepository<PollVote> pollRepository,
    IRepository<User> userRepository,
    IRealTimeService realTimeService) : IPollService
{
    public async Task<List<PollVoteOutput>> GetAllPollVoteAsync(Guid pollReminderId)
    {
        logger.LogInformation("Get all poll votes");

        var votes = await pollRepository.Query()
            .Where(v => v.PollReminderId == pollReminderId)
            .ToListAsync();

        logger.LogInformation($"Succes : All {votes.Count} votes found");

        return votes.Select(v => v.ToOutput()).ToList();
    }

    public async Task<Guid> AddPollVoteAsync(PollVoteInput vote)
    {
        logger.LogInformation("Add poll vote");
        
        Guid guid = Guid.Empty;
        PollVote? pollVote;

        if (await userRepository.Query().AnyAsync(u => u.Id == vote.VotedBy))
        {
            pollVote = await pollRepository.Query()
                .Include(v => v.PollReminder)
                .FirstOrDefaultAsync(v => v.PollReminderId == vote.ReminderId && v.VotedBy == vote.VotedBy);

            if (pollVote is null)
                throw new Exception("User doesnt exist");

            pollVote.Choice = vote.Choice;
            pollVote.VotedAt = DateTime.UtcNow;
            guid = pollVote.Id;

            pollRepository.Update(pollVote);
            await pollRepository.SaveChangesAsync();

            var colocationId = await pollRepository.Query()
                .Where(v => v.Id == guid)
                .Select(v => v.PollReminder.ColocationId)
                .FirstOrDefaultAsync();
            await realTimeService.SendToGroupAsync(colocationId, "UpdatePollVote", pollVote.ToOutput());
        }
        else
        {
            pollVote = new PollVote
            {
                Id = Guid.NewGuid(),
                PollReminderId = vote.ReminderId,
                VotedAt = DateTime.UtcNow,
                VotedBy = vote.VotedBy,
                Choice = vote.Choice
            };

            guid = pollVote.Id;

            await pollRepository.AddAsync(pollVote);
            logger.LogInformation($"Succesfully added poll vote {pollVote.Id}");
            await pollRepository.SaveChangesAsync();

            var colocationId = await pollRepository.Query()
                .Where(v => v.Id == guid)
                .Select(v => v.PollReminder.ColocationId)
                .FirstOrDefaultAsync();
            await realTimeService.SendToGroupAsync(colocationId, "NewPollVote", pollVote.ToOutput());
        }

        logger.LogInformation($"Succesfully processed poll vote {guid}");

        return guid;
    }

    public async Task<Guid> DeletePollVoteAsync(Guid id)
    {
        var pollVote = await pollRepository.Query()
            .Include(v => v.PollReminder)
            .FirstOrDefaultAsync(v => v.Id == id);

        if (pollVote == null)
            throw new Exception("Poll vote not found");

        logger.LogInformation($"Delete poll vote {id}");

        pollRepository.Delete(pollVote);
        await pollRepository.SaveChangesAsync();

        await realTimeService.SendToGroupAsync(pollVote.PollReminder.ColocationId, "DeletePollVote", id);

        logger.LogInformation($"Succesfully deleted poll vote {id}");

        return pollVote.Id;
    }
}

