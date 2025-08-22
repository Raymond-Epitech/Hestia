using EntityFramework.Models;
using Shared.Models.DTO;
using Shared.Models.Input;
using Shared.Models.Output;
using Shared.Models.Update;

namespace Business.Mappers
{
    public static class ReminderMappers
    {
        public static ReminderOutput ToOutput(this Reminder reminder)
        {
            return reminder switch
            {
                TextReminder textReminder => new TextReminderOutput
                {
                    Id = textReminder.Id,
                    CreatedBy = textReminder.CreatedBy,
                    CreatedAt = textReminder.CreatedAt,
                    LinkToPP = UserMapper.GetLinkToPP(textReminder.CreatedBy),
                    Content = textReminder.Content,
                    Color = textReminder.Color,
                    CoordX = textReminder.CoordX,
                    CoordY = textReminder.CoordY,
                    CoordZ = textReminder.CoordZ
                },
                ImageReminder imageReminder => new ImageReminderOutput
                {
                    Id = imageReminder.Id,
                    CreatedBy = imageReminder.CreatedBy,
                    CreatedAt = imageReminder.CreatedAt,
                    LinkToPP = UserMapper.GetLinkToPP(imageReminder.CreatedBy),
                    ImageUrl = imageReminder.ImageUrl,
                    Color = imageReminder.Color,
                    CoordX = imageReminder.CoordX,
                    CoordY = imageReminder.CoordY,
                    CoordZ = imageReminder.CoordZ
                },
                ShoppingListReminder shoppingListReminder => new ShoppingListReminderOutput
                {
                    Id = shoppingListReminder.Id,
                    CreatedBy = shoppingListReminder.CreatedBy,
                    CreatedAt = shoppingListReminder.CreatedAt,
                    LinkToPP = UserMapper.GetLinkToPP(shoppingListReminder.CreatedBy),
                    Name = shoppingListReminder.Name,
                    Items = shoppingListReminder.ShoppingItems?.Select(si => si.ToOutput()).ToList() ?? new List<ShoppingItemOutput>(),
                    CoordX = shoppingListReminder.CoordX,
                    CoordY = shoppingListReminder.CoordY,
                    CoordZ = shoppingListReminder.CoordZ
                },
                PollReminder pollReminder => new PollReminderOutput
                {
                    Id = pollReminder.Id,
                    CreatedBy = pollReminder.CreatedBy,
                    CreatedAt = pollReminder.CreatedAt,
                    LinkToPP = UserMapper.GetLinkToPP(pollReminder.CreatedBy),
                    Title = pollReminder.Title,
                    Description = pollReminder.Description,
                    ExpirationDate = pollReminder.ExpirationDate,
                    IsAnonymous = pollReminder.IsAnonymous,
                    AllowMultipleChoices = pollReminder.AllowMultipleChoices,
                    Votes = pollReminder.PollVotes?.Select(v => v.ToOutput()).ToList() ?? new List<PollVoteOutput>(),
                    CoordX = pollReminder.CoordX,
                    CoordY = pollReminder.CoordY,
                    CoordZ = pollReminder.CoordZ
                },
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
