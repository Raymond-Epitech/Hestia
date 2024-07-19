using Business.Models.DTO;
using Business.Models.Output;

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
    }
}
