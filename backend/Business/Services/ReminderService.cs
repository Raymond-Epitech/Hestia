using Business.Interfaces;
using Business.Mappers;
using EntityFramework.Models;
using EntityFramework.Repositories;
using LazyCache;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Shared.Exceptions;
using Shared.Models.DTO;
using Shared.Models.Input;
using Shared.Models.Output;
using Shared.Models.Update;

namespace Business.Services;

public class ReminderService(ILogger<ReminderService> logger,
    IRepository<Reminder> reminderRepository,
    IRealTimeService realTimeService,
    IAppCache cache) : IReminderService
{
    private string ImageRoute => "wwwroot/uploads/";

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
            .Select(r => new ReminderOutput
            {
                Id = r.Id,
                Content = r.Content,
                Color = r.Color,
                CreatedBy = r.CreatedBy,
                CreatedAt = r.CreatedAt,
                IsImage = r.IsImage,
                CoordX = r.CoordX,
                CoordY = r.CoordY,
                CoordZ = r.CoordZ
            })
            .ToListAsync();

            logger.LogInformation("Succes : All reminders found");

            return reminders;
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
            .Select(r => new ReminderOutput
            {
                Id = r.Id,
                Content = r.Content,
                Color = r.Color,
                CreatedBy = r.CreatedBy,
                CreatedAt = r.CreatedAt,
                IsImage = r.IsImage,
                CoordX = r.CoordX,
                CoordY = r.CoordY,
                CoordZ = r.CoordZ
            })
            .FirstOrDefaultAsync();

        if (reminder == null)
        {
            throw new NotFoundException($"Reminder {id} not found");
        }

        logger.LogInformation("Succes : Reminder found");
            
        return reminder;
    }

    /// <summary>
    /// Get the content type of a file based on its extension
    /// </summary>
    /// <param name="path">The path of the file to get the content type</param>
    /// <returns>The correct content path</returns>
    private string GetContentType(string path)
    {
        var ext = Path.GetExtension(path).ToLowerInvariant();
        return ext switch
        {
            ".jpg" or ".jpeg" => "image/jpeg",
            ".png" => "image/png",
            ".gif" => "image/gif",
            ".webp" => "image/webp",
            ".pdf" => "application/pdf",
            ".txt" => "text/plain",
            _ => "application/octet-stream"
        };
    }

    /// <summary>
    /// Get an image by its name
    /// </summary>
    /// <param name="fileName">The name of the file</param>
    /// <returns>The file's byte and its parameters</returns>
    /// <exception cref="InvalidDataException"></exception>
    /// <exception cref="NotFoundException"></exception>
    public async Task<FileDTO> GetImageByNameAsync(string fileName)
    {
        if (string.IsNullOrWhiteSpace(fileName))
            throw new InvalidDataException("File name invalid");

        var filePath = Path.Combine(Directory.GetCurrentDirectory(), ImageRoute, fileName);

        if (!File.Exists(filePath))
            throw new NotFoundException("Image not found");

        var file = new FileDTO
        {
            FileName = fileName,
            ContentType = GetContentType(filePath),
            Content = await File.ReadAllBytesAsync(filePath)
        };
        
        if (file.Content == null)
        {
            throw new NotFoundException("Image not found");
        }

        return file;
    }

    /// <summary>
    /// Save an image to the server
    /// </summary>
    /// <param name="file">The file</param>
    /// <returns>the new name of the file</returns>
    private async Task<string> SaveImage(IFormFile file)
    {
        var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), ImageRoute);

        if (!Directory.Exists(uploadsFolder))
            Directory.CreateDirectory(uploadsFolder);

        var uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);

        var filePath = Path.Combine(uploadsFolder, uniqueFileName);

        using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await file.CopyToAsync(stream);
        }

        return uniqueFileName;
    }

    /// <summary>
    /// Delete an image from the server
    /// </summary>
    /// <param name="fileName">The file name</param>
    /// <returns>The file name</returns>
    /// <exception cref="InvalidDataException"></exception>
    public string DeleteImage(string fileName)
    {
        var filePath = Path.Combine(Directory.GetCurrentDirectory(), ImageRoute, fileName);

        if (!File.Exists(filePath))
            throw new NotFoundException("Image not found");

        File.Delete(filePath);

        return fileName;
    }

    /// <summary>
    /// Add a reminder
    /// </summary>
    /// <param name="input">The reminder class with all info of a reminder</param>
    /// <exception cref="ContextException">An error has occured while adding reminder from db</exception>
    public async Task<Guid> AddReminderAsync(ReminderInput input)
    {
        var fileName = "";
        if (input.IsImage)
            fileName = await SaveImage(input.Image!);
        
        var reminder = input.ToDb(fileName);

        try
        {
            await reminderRepository.AddAsync(reminder);
            await reminderRepository.SaveChangesAsync();
        }
        catch
        {
            if (input.IsImage)
                DeleteImage(fileName);
        }

        cache.Remove($"reminders:{reminder.ColocationId}");

        var reminderOutput = new ReminderOutput
        {
            Id = reminder.Id,
            Content = reminder.Content,
            Color = reminder.Color,
            IsImage = reminder.IsImage,
            CreatedBy = reminder.CreatedBy,
            CreatedAt = reminder.CreatedAt,
            CoordX = reminder.CoordX,
            CoordY = reminder.CoordY,
            CoordZ = reminder.CoordZ
        };
        await realTimeService.SendToGroupAsync(reminder.ColocationId, "NewReminderAdded", reminderOutput);

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
        var reminder = await reminderRepository.GetByIdAsync(input.Id);
            
        if (reminder == null)
        {
            throw new NotFoundException($"Reminder {input.Id} not found");
        }

        reminder.UpdateFromInput(input);
        reminderRepository.Update(reminder);
        await reminderRepository.SaveChangesAsync();

        cache.Remove($"reminders:{reminder.ColocationId}");

        var reminderOutput = new ReminderOutput
        {
            Id = reminder.Id,
            Content = reminder.Content,
            Color = reminder.Color,
            CreatedBy = reminder.CreatedBy,
            CreatedAt = reminder.CreatedAt,
            CoordX = reminder.CoordX,
            CoordY = reminder.CoordY,
            CoordZ = reminder.CoordZ
        };
        await realTimeService.SendToGroupAsync(reminder.ColocationId, "ReminderUpdated", reminderOutput);

        logger.LogInformation("Succes : Reminder updated");

        return reminder.Id;
    }

    /// <summary>
    /// Update a range of reminders
    /// </summary>
    /// <param name="inputs">a dictionary with the id, and the value of the reminders</param>
    /// <exception cref="NotFoundException">One or more reminder where not found with those id</exception>
    /// <exception cref="ContextException">An error has occured while adding one or more reminder from db</exception>
    public async Task<int> UpdateRangeReminderAsync(List<ReminderUpdate> inputs)
    {
        var idsToMatch = inputs.Select(x => x.Id).ToList();
        var reminders = await reminderRepository.Query()
            .Where(r => idsToMatch.Contains(r .Id))
            .ToListAsync();
            
        if (reminders.Count == 0)
        {
            throw new NotFoundException($"No reminders found");
        }
        
        var notfounds = inputs.Select(x => x.Id).Except(reminders.Select(x => x.Id)).ToList();
            
        if (notfounds.Any())
        {
            throw new NotFoundException($"Reminders {String.Join(", ", notfounds)} was/were not found");
        }

        using (var transaction = await reminderRepository.BeginTransactionAsync())
        {
            try
            {
                logger.LogInformation("Transaction begin");
                    
                foreach (var reminder in reminders)
                {
                    var input = inputs.FirstOrDefault(x => x.Id == reminder.Id);
                        
                    if (input is null)
                    {
                        throw new NotFoundException($"input {reminder.Id} not found");
                    }

                    reminder.UpdateFromInput(input);
                }
                    
                await reminderRepository.SaveChangesAsync();

                cache.Remove($"reminders:{reminders.First().ColocationId}");

                transaction.Commit();

                logger.LogInformation("Transaction commit");
            }
            catch (NotFoundException)
            {
                logger.LogError("Reminder not found, Transaction rollbacked");
                transaction.Rollback();
                throw;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occurred while updating the reminder from the db, Transaction rollbacked");
                transaction.Rollback();
                throw new ContextException("An error occurred while updating the reminder from the db", ex);
            }
        }

        cache.Remove($"reminders:{reminders.FirstOrDefault()!.ColocationId}");

        var reminderOutputs = reminders.Select(r => new ReminderOutput
        {
            Id = r.Id,
            Content = r.Content,
            Color = r.Color,
            CreatedBy = r.CreatedBy,
            CreatedAt = r.CreatedAt,
            CoordX = r.CoordX,
            CoordY = r.CoordY,
            CoordZ = r.CoordZ
        }).ToList();
        await realTimeService.SendToGroupAsync(reminders.First().ColocationId, "RemindersUpdated", reminderOutputs); ;

        logger.LogInformation("Succes : Reminders all updated");

        return reminders.Count;
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
