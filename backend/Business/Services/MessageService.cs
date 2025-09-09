using Business.Interfaces;
using EntityFramework.Models;
using EntityFramework.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Shared.Models.Input;
using Shared.Models.Output;
using Shared.Models.Update;

namespace Business.Services;

public class MessageService(
    ILogger<MessageService> logger,
    IRepository<Message> repository,
    IRepository<User> userRepository,
    IRealTimeService realTimeService,
    IFirebaseNotificationService notificationService) : IMessageService
{
    public async Task<List<MessageOutput>> GetAllMessagesAsync(Guid colocationId)
    {
        var messages = await repository.Query()
            .Where(m => m.ColocationId == colocationId)
            .Select(m => new MessageOutput
            {
                Id = m.Id,
                ColocationId = m.ColocationId,
                Content = m.Content,
                SendAt = m.SentAt,
                SendBy = m.SentBy
            })
            .OrderBy(m => m.SendAt)
            .ToListAsync();

        logger.LogInformation($"Retrieved {messages.Count} messages for colocation {colocationId}");

        return messages;
    }

    public async Task<MessageOutput> GetMessageAsync(Guid id)
    {
        var message = await repository.Query()
            .Where(m => m.Id == id)
            .Select(m => new MessageOutput
            {
                Id = m.Id,
                ColocationId = m.ColocationId,
                Content = m.Content,
                SendAt = m.SentAt,
                SendBy = m.SentBy
            })
            .FirstOrDefaultAsync();

        if (message == null)
        {
            throw new KeyNotFoundException($"Message with Id {id} not found");
        }

        logger.LogInformation($"Retrieved message with Id {id}");

        return message;
    }

    public async Task<Guid> AddMessageAsync(MessageInput input)
    {
        var message = new Message
        {
            Id = Guid.NewGuid(),
            ColocationId = input.ColocationId,
            Content = input.Content,
            SentBy = input.SendBy
        };

        await repository.AddAsync(message);
        await repository.SaveChangesAsync();

        logger.LogInformation($"Added new message with Id {message.Id} for colocation {input.ColocationId}");

        var messageOutput = new MessageOutput
        {
            Id = message.Id,
            ColocationId = message.ColocationId,
            Content = message.Content,
            SendAt = message.SentAt,
            SendBy = message.SentBy
        };
        await realTimeService.SendToGroupAsync(messageOutput.ColocationId, "NewMessageAdded", messageOutput);
        var name = userRepository.Query()
            .Where(u => u.Id == message.SentBy)
            .Select(u => u.Username)
            .FirstOrDefault();
        await notificationService.SendNotificationToColocationAsync(new NotificationInput {
            Id = message.ColocationId,
            Title = $"New message from {name} !",
            Body = messageOutput.Content,
        },
        messageOutput.SendBy);

        logger.LogInformation($"Sent real-time notification for new message with Id {message.Id}");

        return message.Id;
    }

    public async Task<Guid> UpdateMessageAsync(MessageUpdate input)
    {
        var message = await repository.GetByIdAsync(input.Id);

        if (message == null)
        {
            throw new KeyNotFoundException($"Message with Id {input.Id} not found");
        }

        message.Content = input.Content;
        message.ColocationId = input.ColocationId;
        message.SentBy = input.SendBy;
        repository.Update(message);
        await repository.SaveChangesAsync();

        logger.LogInformation($"Updated message with Id {input.Id}");

        var messageOutput = new MessageOutput
        {
            Id = message.Id,
            ColocationId = message.ColocationId,
            Content = message.Content,
            SendAt = message.SentAt,
            SendBy = message.SentBy
        };
        await realTimeService.SendToGroupAsync(messageOutput.ColocationId, "MessageUpdated", messageOutput);

        logger.LogInformation($"Sent real-time notification for updated message with Id {message.Id}");

        return message.Id;
    }

    public async Task<Guid> DeleteMessageAsync(Guid id)
    {
        var message = await repository.GetByIdAsync(id);

        if (message == null)
        {
            throw new KeyNotFoundException($"Message with Id {id} not found");
        }

        repository.Delete(message);
        await repository.SaveChangesAsync();

        logger.LogInformation($"Deleted message with Id {id}");

        await realTimeService.SendToGroupAsync(message.ColocationId, "MessageDeleted", id);

        return id;
    }
}

