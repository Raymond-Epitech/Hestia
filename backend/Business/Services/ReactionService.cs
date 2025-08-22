using Business.Interfaces;
using Business.Mappers;
using EntityFramework.Models;
using EntityFramework.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Shared.Models.Input;
using Shared.Models.Output;

namespace Business.Services;

public class ReactionService(ILogger<ReactionService> logger,
    IRepository<Reaction> reactionRepository,
    IRealTimeService realTimeService,
    IRepository<Reminder> reminderRepository) : IReactionService
{
    public async Task<List<ReactionOutput>> GetReactionsByPostIdAsync(Guid ReminderId)
    {
        logger.LogInformation($"Fetching reactions for ReminderId: {ReminderId}");

        return await reactionRepository.Query()
            .Where(r => r.ReminderId == ReminderId)
            .Select(r => r.ToOutput())
            .ToListAsync();
    }

    public async Task<Guid> AddReactionAsync(ReactionInput reactionInput)
    {
        var reaction = await reactionRepository.Query()
            .Include(r => r.Reminder)
            .FirstOrDefaultAsync(r => r.UserId == reactionInput.UserId && r.ReminderId == reactionInput.ReminderId);
        var add = true;

        if (reaction is not null)
        {
            reaction.Type = reactionInput.Type;
            add = false;
            reactionRepository.Update(reaction);
        }
        else
        {
            reaction = new Reaction
            {
                Id = Guid.NewGuid(),
                ReminderId = reactionInput.ReminderId,
                UserId = reactionInput.UserId,
                Type = reactionInput.Type
            };
            await reactionRepository.AddAsync(reaction);
        }

        await reactionRepository.SaveChangesAsync();

        if (add)
        {
            await realTimeService.SendToGroupAsync(reaction.Reminder.ColocationId, "NewReaction", reaction.ToOutput());
        }
        else
        {
            await realTimeService.SendToGroupAsync(reaction.Reminder.ColocationId, "UpdateReaction", reaction.ToOutput());
        }

        logger.LogInformation($"Added reaction: {reaction.Type} for ReminderId: {reaction.ReminderId} by UserId: {reaction.UserId}");

        return reaction.ReminderId;
    }

    public async Task<Guid> DeleteReactionAsync(ReactionInputForDelete input)
    {
        var reaction = await reactionRepository.Query()
            .Include(r => r.Reminder)
            .FirstOrDefaultAsync(r => r.UserId == input.UserId && r.ReminderId == input.ReminderId);

        if (reaction is null)
        {
            logger.LogWarning($"No reaction found for UserId: {input.UserId} and ReminderId: {input.ReminderId}");
            throw new KeyNotFoundException("Reaction not found");
        }

        reactionRepository.Delete(reaction);
        await reactionRepository.SaveChangesAsync();

        await realTimeService.SendToGroupAsync(reaction.Reminder.ColocationId, "DeleteReaction", reaction.Id);

        logger.LogInformation($"Deleted reaction for UserId: {input.UserId} and ReminderId: {input.ReminderId}");

        return reaction.ReminderId;
    }
}

