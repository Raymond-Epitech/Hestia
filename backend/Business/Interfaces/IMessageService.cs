using Shared.Models.Input;
using Shared.Models.Output;
using Shared.Models.Update;

namespace Business.Interfaces;

public interface IMessageService
{
    Task<List<MessageOutput>> GetAllMessagesAsync(Guid colocationId);
    Task<MessageOutput> GetMessageAsync(Guid id);
    Task<Guid> AddMessageAsync(MessageInput input);
    Task<Guid> UpdateMessageAsync(MessageUpdate input);
    Task<Guid> DeleteMessageAsync(Guid id);
}

