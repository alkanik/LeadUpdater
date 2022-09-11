using Polly;
using Polly.Retry;

namespace LeadUpdater.Policies;

public class ClientPolicy
{
    public AsyncRetryPolicy<HttpResponseMessage> RetryPolicy { get; }

    public ClientPolicy()
    {
        RetryPolicy = Policy.HandleResult<HttpResponseMessage>(
            res => !res.IsSuccessStatusCode)
            .WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));
    }
}