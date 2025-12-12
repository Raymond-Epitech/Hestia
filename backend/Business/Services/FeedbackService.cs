using Business.Interfaces;
using EntityFramework.Models;
using EntityFramework.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Shared.Models.Input;

namespace Business.Services;

public class FeedbackService(ILogger<ChoreService> logger,
    IRepository<Feedback> feedbackRepository,
    IImageService imageService) : IFeedbackService
{
    public async Task<List<FeedbackOutput>> GetAllFeedbacks()
    {
        logger.LogInformation("Retrieving all feedback entries from the database.");

        return await feedbackRepository.Query().Select(f => new FeedbackOutput
        {
            Id = f.Id,
            CreatedBy = f.CreatedBy,
            CreatedAt = f.CreatedAt,
            VersionHash = f.VersionHash,
            Title = f.Title,
            Description = f.Description,
            BugType = f.BugType,
            AttachmentPath = f.AttachmentPath
        }).ToListAsync();
    }

    public async Task<Guid> AddFeedback(FeedbackInput input)
    {
        var feedback = new Feedback
        {
            Id = Guid.NewGuid(),
            CreatedBy = input.CreatedBy,
            CreatedAt = DateTime.UtcNow,
            VersionHash = input.VersionHash,
            Title = input.Title,
            Description = input.Description,
            BugType = input.BugType,
            AttachmentPath = input.Attatchment != null
                ? await imageService.SaveImage(input.Attatchment)
                : null
        };

        await feedbackRepository.AddAsync(feedback);
        await feedbackRepository.SaveChangesAsync();

        logger.LogInformation($"Feedback {feedback.Id} added by user {feedback.CreatedBy}.");

        return feedback.Id;
    }
}

