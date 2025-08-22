using Business.Interfaces;
using EntityFramework.Models;
using EntityFramework.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Shared.Models.Input;

namespace Business.Services;

public class ReactionService(ILogger<ReactionService> logger,
    IRepository<Reaction> reactionRepository) : IReactionService
{
    public async Task<List<string>> GetReactionsByPostIdAsync(Guid ReminderId)
    {
        logger.LogInformation($"Fetching reactions for ReminderId: {ReminderId}");

        return await reactionRepository.Query()
            .Where(r => r.ReminderId == ReminderId)
            .Select(r => r.Type)
            .ToListAsync();
    }

    public async Task<Guid> AddReactionAsync(ReactionInput reactionInput)
    {
        var reaction = await reactionRepository.Query().FirstOrDefaultAsync(r => r.UserId == reactionInput.UserId && r.ReminderId == reactionInput.ReminderId);
        if (reaction is not null)
        {
            reaction.Type = reactionInput.Type;
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

        logger.LogInformation($"Adding reaction: {reaction.Type} for ReminderId: {reaction.ReminderId} by UserId: {reaction.UserId}");

        await reactionRepository.SaveChangesAsync();

        logger.LogInformation("Reaction added successfully");

        return reaction.ReminderId;
    }

    public async Task<Guid> DeleteReactionAsync(ReactionInputForDelete input)
    {
        var reaction = await reactionRepository.Query().FirstOrDefaultAsync(r => r.UserId == input.UserId && r.ReminderId == input.ReminderId);

        if (reaction is null)
        {
            logger.LogWarning($"No reaction found for UserId: {input.UserId} and ReminderId: {input.ReminderId}");
            throw new KeyNotFoundException("Reaction not found");
        }

        reactionRepository.Delete(reaction);
        await reactionRepository.SaveChangesAsync();

        logger.LogInformation($"Deleted reaction for UserId: {input.UserId} and ReminderId: {input.ReminderId}");

        return reaction.ReminderId;
    }
}

