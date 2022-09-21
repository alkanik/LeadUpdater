using Cronos;
using LeadUpdater.Infrastructure;

namespace LeadUpdater;

public class Scheduler : IScheduler
{
    private readonly CronExpression _cronJob;

    public Scheduler()
    {
        _cronJob = CronExpression.Parse(Constant.CronExpression, CronFormat.IncludeSeconds); ;
    }

    public TimeSpan GetDelayTimeSpan()
    {
        var now = DateTime.UtcNow;
        var nextUtc = _cronJob.GetNextOccurrence(now);
        var delayTimeSpan = (nextUtc.Value - now);
        return delayTimeSpan;
    }
}