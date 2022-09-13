namespace LeadUpdater;

public interface IReportingClient
{
    Task<List<int>> GetCelebrantsFromDateToNow(int daysCount, CancellationToken token);
    Task<List<int>> GetLeadIdsWithNecessaryTransactionsCount(int transactionsCount, int daysCount, CancellationToken token);
    Task<List<int>> GetLeadsIdsWithNecessaryAmountDifference(decimal amountDifference, int daysCount, CancellationToken token);
}