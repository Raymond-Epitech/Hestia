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
    IRepository<User> userRepository) : IPollService
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

        if (await userRepository.Query().AnyAsync(u => u.Id == vote.VotedBy))
        {
            var existingVote = await pollRepository.Query()
                .FirstOrDefaultAsync(v => v.PollReminderId == vote.ReminderId && v.VotedBy == vote.VotedBy);

            if (existingVote is null)
                throw new Exception("User doesnt exist");

            existingVote.Choice = vote.Choice;
            existingVote.VotedAt = DateTime.UtcNow;
            guid = existingVote.Id;

            pollRepository.Update(existingVote);
        }
        else
        {
            var pollVote = new PollVote
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
        }

        await pollRepository.SaveChangesAsync();

        return guid;
    }

    public async Task<Guid> UpdatePollVoteAsync(PollVoteUpdate vote)
    {
        var pollVote = await pollRepository.GetByIdAsync(vote.Id);

        if (pollVote == null)
            throw new Exception("Poll vote not found");

        logger.LogInformation($"Update poll vote {vote.Id}");

        pollVote.Choice = vote.Choice;
        pollVote.VotedAt = vote.VotedAt;

        pollRepository.Update(pollVote);
        await pollRepository.SaveChangesAsync();

        logger.LogInformation($"Succesfully updated poll vote {pollVote.Id}");

        return pollVote.Id;
    }

    public async Task<Guid> DeletePollVoteAsync(Guid id)
    {
        var pollVote = await pollRepository.GetByIdAsync(id);

        if (pollVote == null)
            throw new Exception("Poll vote not found");

        logger.LogInformation($"Delete poll vote {id}");

        pollRepository.Delete(pollVote);
        await pollRepository.SaveChangesAsync();

        logger.LogInformation($"Succesfully deleted poll vote {id}");

        return pollVote.Id;
    }
}

