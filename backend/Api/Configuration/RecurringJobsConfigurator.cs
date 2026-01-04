using System.Runtime.InteropServices;
using Hangfire;
using Business.Services;

public class RecurringJobsConfigurator
{
    private readonly IRecurringJobManager _jobs;

    public RecurringJobsConfigurator(IRecurringJobManager jobs) => _jobs = jobs;

    private static TimeZoneInfo ResolveParisTz()
    {
        try
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                return TimeZoneInfo.FindSystemTimeZoneById("Romance Standard Time");
            return TimeZoneInfo.FindSystemTimeZoneById("Europe/Paris");
        }
        catch
        {
            return TimeZoneInfo.Local;
        }
    }

    public void Configure()
    {
        var tz = ResolveParisTz();

        _jobs.AddOrUpdate<ExpenseService>(
            "add-recurrent-expenses",
            s => s.TriggerAllExpenseAutomation(),
            Cron.Daily(3, 0),
            new RecurringJobOptions { TimeZone = tz }
        );

        _jobs.Trigger("add-recurrent-expenses");
    }
}
