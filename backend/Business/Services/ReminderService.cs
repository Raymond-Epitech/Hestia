using Business.Interfaces;
using Business.Mappers;
using EntityFramework.Models;
using EntityFramework.Repositories;
using LazyCache;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Shared.Enums;
using Shared.Exceptions;
using Shared.Models.Input;
using Shared.Models.Output;
using Shared.Models.Update;
using SixLabors.ImageSharp;

namespace Business.Services;

public class ReminderService(ILogger<ReminderService> logger,
    IRepository<Reminder> reminderRepository,
    IRealTimeService realTimeService,
    IFirebaseNotificationService notificationService,
    IRepository<User> userRepository,
    IImageService imageService,
    IAppCache cache) : IReminderService
{

    /// <summary>
    /// Get all reminders
    /// </summary>
    /// <returns>All the reminders available</returns>
    /// <exception cref="ContextException">An error has occured while retriving the reminders from db</exception>
    public async Task<List<ReminderOutput>> GetAllRemindersAsync(Guid colocationId)
    {
        var cacheKey = $"reminders:{colocationId}";

        return await cache.GetOrAddAsync(cacheKey, async entry =>
        {
            entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(1);

            var reminders = await reminderRepository.Query()
                .Where(r => r.ColocationId == colocationId)
                .Include(r => r.Reactions)
                .Include(r => (r as ShoppingListReminder)!.ShoppingItems)
                .Include(r => (r as PollReminder)!.PollVotes)
                .Include(r => r.User)
                .Include(r => r.Reactions)
                .ToListAsync();

            logger.LogInformation($"Succes : All {reminders.Count} reminders found");

            return reminders.Select(r => r.ToOutput()).ToList();
        });
    }

    /// <summary>
    /// Get a reminder
    /// </summary>
    /// <param name="id">the Guid of the reminder</param>
    /// <returns>The found reminder</returns>
    /// <exception cref="NotFoundException">The reminder was not found</exception>
    /// <exception cref="ContextException">An error has occured while retriving reminder from db</exception>
    public async Task<ReminderOutput> GetReminderAsync(Guid id)
    {
        var reminder = await reminderRepository.Query()
            .Where(r => r.Id == id)
            .Include(r => r.Reactions)
            .Include(r => (r as ShoppingListReminder)!.ShoppingItems)
            .Include(r => (r as PollReminder)!.PollVotes)
            .Include(r => r.User)
            .Include(r => r.Reactions)
            .FirstOrDefaultAsync();

        if (reminder == null)
        {
            throw new NotFoundException($"Reminder {id} not found");
        }

        logger.LogInformation("Succes : Reminder found");
            
        return reminder.ToOutput();
    }

    /// <summary>
    /// Add a reminder
    /// </summary>
    /// <param name="input">The reminder class with all info of a reminder</param>
    /// <exception cref="ContextException">An error has occured while adding reminder from db</exception>
    public async Task<Guid> AddReminderAsync(ReminderInput input)
    {
        var reminder = input.ToDb();

        if (reminder is ImageReminder imageReminder && input.File is not null)
        {
            imageReminder.ImageUrl = await imageService.SaveImage(input.File);
        }

        var user = await userRepository.GetByIdAsync(reminder.CreatedBy);

        if (user == null)
        {
            throw new NotFoundException($"User {reminder.CreatedBy} not found");
        }
        reminder.User = user!;

        try
        {
            await reminderRepository.AddAsync(reminder);
            await reminderRepository.SaveChangesAsync();
        }
        catch
        {
            if (reminder is ImageReminder image)
            {
                imageService.DeleteImage(image.ImageUrl);
            }
        }

        cache.Remove($"reminders:{reminder.ColocationId}");

        await realTimeService.SendToGroupAsync(reminder.ColocationId, "NewReminderAdded", reminder.ToOutput());
        await notificationService.SendNotificationToColocationAsync(new NotificationInput
        {
            Id = reminder.ColocationId,
            Title = "New reminder",
            Body = $"{reminder.User.Username} added a new reminder"
        }, reminder.CreatedBy);

        logger.LogInformation("Succes : Reminder added");
            
        return reminder.Id;
    }

    /// <summary>
    /// Update a reminder
    /// </summary>
    /// <param name="input">The reminder class with all info of a reminder</param>
    /// <exception cref="MissingArgumentException">The Id of the reminder is missing</exception>
    /// <exception cref="NotFoundException">No reminder where found with this id</exception>
    /// <exception cref="ContextException">An error has occured while adding reminder from db</exception>
    public async Task<Guid> UpdateReminderAsync(ReminderUpdate input)
    {
        Reminder? reminder;

        switch (input.ReminderType)
        {
            case ReminderType.Text:
                reminder = await reminderRepository.Query()
                    .Where(r => r.Id == input.Id && r is TextReminder)
                    .Include(r => r.Reactions)
                    .Include(r => r.User)
                    .FirstOrDefaultAsync();
                break;
            case ReminderType.Image:
                throw new InvalidDataException("Cannot update image reminder");
            case ReminderType.ShoppingList:
                reminder = await reminderRepository.Query()
                    .Where(r => r.Id == input.Id && r is ShoppingListReminder)
                    .Include(r => (r as ShoppingListReminder)!.ShoppingItems)
                    .Include(r => r.Reactions)
                    .Include(r => r.User)
                    .FirstOrDefaultAsync();
                break;
            case ReminderType.Poll:
                reminder = await reminderRepository.Query()
                    .Where(r => r.Id == input.Id && r is PollReminder)
                    .Include(r => (r as PollReminder)!.PollVotes)
                    .Include(r => r.Reactions)
                    .Include(r => r.User)
                    .FirstOrDefaultAsync();
                break;
            default:
                throw new MissingArgumentException("Type is invalid or missing");
        }

        if (reminder == null)
        {
            throw new NotFoundException($"Reminder {input.Id} not found");
        }

        reminder.UpdateFromInput(input);
        reminderRepository.Update(reminder);

        await reminderRepository.SaveChangesAsync();

        cache.Remove($"reminders:{reminder.ColocationId}");

        await realTimeService.SendToGroupAsync(reminder.ColocationId, "ReminderUpdated", reminder.ToOutput());

        logger.LogInformation("Succes : Reminder updated");

        return reminder.Id;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="id"></param>
    /// <exception cref="NotFoundException">No reminder where found with this id</exception>
    /// <exception cref="ContextException">An error has occured while adding reminder from db</exception>
    public async Task<Guid> DeleteReminderAsync(Guid id)
    {
        var reminder = await reminderRepository.GetByIdAsync(id);
        if (reminder == null)
        {
            throw new NotFoundException($"Reminder {id} not found");
        }
        reminderRepository.Delete(reminder);
        await reminderRepository.SaveChangesAsync();

        cache.Remove($"reminders:{reminder.ColocationId}");

        await realTimeService.SendToGroupAsync(reminder.ColocationId, "ReminderDeleted", id);

        logger.LogInformation("Succes : Reminder deleted");

        return id;
    }
}
