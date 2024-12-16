using Business.Models.Input;
using Business.Models.Output;
using EntityFramework.Models;

namespace Business.Mappers
{
    public static class ChoreMappers
    {
        public static Chore ToDb(this ChoreInput choreInput)
        {
            return new Chore
            {
                Id = Guid.NewGuid(),
                CollocationId = choreInput.CollocationId,
                CreatedAt = DateTime.Now.ToUniversalTime(),
                DueDate = choreInput.DueDate,
                CreatedBy = choreInput.CreatedBy,
                Title = choreInput.Title,
                Description = choreInput.Description,
                IsDone = choreInput.IsDone
            };
        }

        public static ChoreMessage ToDb(this ChoreMessageInput choreInput)
        {
            return new ChoreMessage
            {
                Id = Guid.NewGuid(),
                CreatedAt = DateTime.Now.ToUniversalTime(),
                Content = choreInput.Content,
                ChoreId = choreInput.ChoreId,
                CreatedBy = choreInput.CreatedBy,
            };
        }

        public static Chore UpdateFromInput(this Chore chore, ChoreUpdate input)
        {
            chore.Id = input.Id;
            chore.CollocationId = input.CollocationId;
            chore.Title = input.Title;
            chore.Description = input.Description;
            chore.DueDate = input.DueDate;
            chore.IsDone = input.IsDone;
            return chore;
        }
    }
}
