using EntityFramework.Models;
using Shared.Models.Input;
using Shared.Models.Output;
using Shared.Models.Update;

namespace Business.Mappers
{
    public static class ChoreMappers
    {
        public static Chore ToDb(this ChoreInput choreInput)
        {
            return new Chore
            {
                Id = Guid.NewGuid(),
                ColocationId = choreInput.ColocationId,
                CreatedAt = DateTime.Now.ToUniversalTime(),
                UpdatedAt = DateTime.Now.ToUniversalTime(),
                DueDate = choreInput.DueDate,
                CreatedBy = choreInput.CreatedBy,
                Title = choreInput.Title,
                Description = choreInput.Description,
                IsDone = choreInput.IsDone
            };
        }

        public static ChoreOutput ToOutput(this Chore c)
        {
            return new ChoreOutput
            {
                Id = c.Id,
                CreatedBy = c.CreatedBy,
                CreatedAt = c.CreatedAt,
                UpdatedAt = c.UpdatedAt,
                DueDate = c.DueDate,
                Title = c.Title,
                Description = c.Description,
                IsDone = c.IsDone,
                EnrolledUsers = c.ChoreEnrollments
                    .ToDictionary(ce => ce.UserId, ce => ce.User.PathToProfilePicture)
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
            chore.ColocationId = input.ColocationId;
            chore.Title = input.Title;
            chore.Description = input.Description;
            chore.DueDate = input.DueDate;
            chore.IsDone = input.IsDone;
            chore.UpdatedAt = DateTime.Now.ToUniversalTime();
            return chore;
        }
    }
}
