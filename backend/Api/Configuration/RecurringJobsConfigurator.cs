using Business.Services;
using Hangfire;

public class RecurringJobsConfigurator
{
    private readonly IRecurringJobManager _jobManager;

    public RecurringJobsConfigurator(IRecurringJobManager jobManager)
    {
        _jobManager = jobManager;
    }

    public void Configure()
    {
        _jobManager.AddOrUpdate<ExpiredChoreRemover>(
            "remove-expired-chores",
            job => job.PurgeOldDataAsync(),
            Cron.Daily,
            new RecurringJobOptions
            {
                TimeZone = TimeZoneInfo.Local
            }
        );
    }
}
