using EntityFramework.Models;
using Shared.Models.DTO;
using Shared.Models.Input;
using Shared.Models.Output;
using Shared.Models.Update;

namespace Business.Mappers
{
    public static class ReminderMappers
    {
        public static ReminderOutput ToOutput(this ReminderDTO reminder)
        {
            return new ReminderOutput
            {
                Id = reminder.Id,
            };
        }

        public static Reminder ToDb(this ReminderInput reminder, string fileName)
        {
            var content = "";
            if (reminder.IsImage)
                content = fileName;
            else
                content = reminder.Content;

            return new Reminder
            {
                Id = Guid.NewGuid(),
                ColocationId = reminder.ColocationId,
                CreatedAt = DateTime.Now.ToUniversalTime(),
                CreatedBy = reminder.CreatedBy,
                Content = content!,
                IsImage = reminder.IsImage,
                Color = reminder.Color,
                CoordX = reminder.CoordX,
                CoordY = reminder.CoordY,
                CoordZ = reminder.CoordZ
            };
        }

        public static Reminder UpdateFromInput(this Reminder reminder, ReminderUpdate input)
        {
            reminder.Color = input.Color;
            reminder.Content = input.Content;
            reminder.CoordX = input.CoordX;
            reminder.CoordY = input.CoordY;
            reminder.CoordZ = input.CoordZ;
            return reminder;
        }
    }
}
