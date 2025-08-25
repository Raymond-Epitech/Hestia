using EntityFramework.Models;
using Shared.Enums;
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
                TextReminder textReminder => new ReminderOutput
                {
                    Id = textReminder.Id,
                    CreatedBy = textReminder.CreatedBy,
                    CreatedAt = textReminder.CreatedAt,
                    LinkToPP = textReminder.User.PathToProfilePicture,
                    ReminderType = ReminderType.Text,
                    Content = textReminder.Content,
                    Color = textReminder.Color,
                    CoordX = textReminder.CoordX,
                    CoordY = textReminder.CoordY,
                    CoordZ = textReminder.CoordZ
                },
                ImageReminder imageReminder => new ReminderOutput
                {
                    Id = imageReminder.Id,
                    CreatedBy = imageReminder.CreatedBy,
                    CreatedAt = imageReminder.CreatedAt,
                    LinkToPP = imageReminder.User.PathToProfilePicture,
                    ReminderType = ReminderType.Image,
                    ImageUrl = imageReminder.ImageUrl,
                    CoordX = imageReminder.CoordX,
                    CoordY = imageReminder.CoordY,
                    CoordZ = imageReminder.CoordZ
                },
                ShoppingListReminder shoppingListReminder => new ReminderOutput
                {
                    Id = shoppingListReminder.Id,
                    CreatedBy = shoppingListReminder.CreatedBy,
                    CreatedAt = shoppingListReminder.CreatedAt,
                    LinkToPP = shoppingListReminder.User.PathToProfilePicture,
                    ReminderType = ReminderType.ShoppingList,
                    ShoppingListName = shoppingListReminder.ShoppingListName,
                    Items = shoppingListReminder.ShoppingItems?.Select(si => si.ToOutput()).ToList() ?? new List<ShoppingItemOutput>(),
                    CoordX = shoppingListReminder.CoordX,
                    CoordY = shoppingListReminder.CoordY,
                    CoordZ = shoppingListReminder.CoordZ
                },
                PollReminder pollReminder => new ReminderOutput
                {
                    Id = pollReminder.Id,
                    CreatedBy = pollReminder.CreatedBy,
                    CreatedAt = pollReminder.CreatedAt,
                    LinkToPP = pollReminder.User.PathToProfilePicture,
                    ReminderType = ReminderType.Poll,
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
                _ => throw new InvalidDataException(),
            };
        }

        public static ShoppingItemOutput ToOutput(this ShoppingItem item)
        {
            return new ShoppingItemOutput
            {
                Id = item.Id,
                Name = item.Name,
                IsChecked = item.IsChecked,
            };
        }

        public static PollVoteOutput ToOutput(this PollVote vote)
        {
            return new PollVoteOutput
            {
                Id = vote.Id,
                VotedAt = vote.VotedAt,
                VotedBy = vote.VotedBy,
                Choice = vote.Choice
            };
        }

        public static ReactionOutput ToOutput(this Reaction reaction)
        {
            return new ReactionOutput
            {
                Id = reaction.Id,
                UserId = reaction.UserId,
                ReminderId = reaction.ReminderId,
                Type = reaction.Type
            };
        }

        public static Reminder ToDb(this ReminderInput input)
        {
            Reminder reminder;

            switch (input.ReminderType)
            {
                case ReminderType.Text:
                    if (input.Content is null || input.Color is null)
                    {
                        throw new ArgumentException("Content and Color are required for Text reminders");
                    }
                    var textReminder = new TextReminder();
                    textReminder.Content = input.Content;
                    textReminder.Color = input.Color;
                    reminder = textReminder;
                    break;

                case ReminderType.Image:
                    var imageReminder = new ImageReminder();
                    imageReminder.ImageUrl = string.Empty;
                    reminder = imageReminder;
                    break;

                case ReminderType.Poll:
                    if (input.PollInput is not { Title: not null,
                        Description: not null,
                        ExpirationDate: not null,
                        AllowMultipleChoices: not null,
                        IsAnonymous: not null})
                    {
                        throw new ArgumentException("PollInput is required for Poll reminders");
                    }
                    var pollReminder = new PollReminder();
                    pollReminder.Title = input.PollInput.Title;
                    pollReminder.Description = input.PollInput.Description;
                    pollReminder.ExpirationDate = input.PollInput.ExpirationDate.Value;
                    pollReminder.IsAnonymous = input.PollInput.IsAnonymous.Value;
                    pollReminder.AllowMultipleChoices = input.PollInput.AllowMultipleChoices.Value;
                    reminder = pollReminder;
                    break;

                case ReminderType.ShoppingList:
                    if (input.ShoppingListName is null || input.ShoppingListName.Length > 50)
                    {
                        throw new ArgumentException("ShoppingListName cannot be longer than 50 characters or be null");
                    }
                    var shoppingReminder = new ShoppingListReminder();
                    shoppingReminder.ShoppingListName = input.ShoppingListName;
                    reminder = shoppingReminder;
                    break;

                default:
                    throw new NotSupportedException($"ReminderType '{input.ReminderType}' not supported");
            }

            reminder.ColocationId = input.ColocationId;
            reminder.CreatedBy = input.CreatedBy;
            reminder.CoordX = input.CoordX;
            reminder.CoordY = input.CoordY;
            reminder.CoordZ = input.CoordZ;
            reminder.CreatedAt = DateTime.UtcNow;

            return reminder;
        }

        public static Reminder UpdateFromInput(this Reminder reminder, ReminderUpdate input)
        {
            reminder.CoordX = input.CoordX;
            reminder.CoordY = input.CoordY;
            reminder.CoordZ = input.CoordZ;

            switch (reminder)
            {
                case TextReminder textReminder:
                    if (input.Content is not null)
                    {
                        textReminder.Content = input.Content;
                    }
                    if (input.Color is not null)
                    {
                        textReminder.Color = input.Color;
                    }
                    break;
                case ImageReminder imageReminder:
                    break;
                case PollReminder pollReminder:
                    if (input.PollInput is not {
                            Title: not null,
                            Description: not null,
                            ExpirationDate: not null,
                            AllowMultipleChoices: not null,
                            IsAnonymous: not null})
                    {
                        pollReminder.Title = input.PollInput!.Title!;
                        pollReminder.Description = input.PollInput.Description;
                        pollReminder.ExpirationDate = input.PollInput.ExpirationDate!.Value;
                        pollReminder.IsAnonymous = input.PollInput.IsAnonymous!.Value;
                        pollReminder.AllowMultipleChoices = input.PollInput.AllowMultipleChoices!.Value;
                    }
                    break;
                case ShoppingListReminder shoppingListReminder:
                    if (input.ShoppingListName is null || input.ShoppingListName.Length > 50)
                    {
                        throw new ArgumentException("ShoppingListName cannot be longer than 50 characters");
                    }
                    shoppingListReminder.ShoppingListName = input.ShoppingListName;
                    break;
                default:
                    throw new NotSupportedException($"Reminder type '{reminder.GetType().Name}' not supported for update");
            }

            return reminder;
        }
    }
}
