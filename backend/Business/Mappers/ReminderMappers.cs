using EntityFramework.Models;
using Shared.Models.DTO;
using Shared.Models.Input;
using Shared.Models.Output;
using Shared.Models.Update;

namespace Shared.Mappers
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

        public static Reminder ToDb(this ReminderInput reminder)
        {
            return new Reminder
            {
                Id = Guid.NewGuid(),
                CollocationId = reminder.CollocationId,
                CreatedAt = DateTime.Now.ToUniversalTime(),
                CreatedBy = reminder.CreatedBy,
                Content = reminder.Content,
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
