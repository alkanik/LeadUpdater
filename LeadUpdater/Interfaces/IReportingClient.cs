namespace LeadUpdater.Business
{
    public interface IReportingClient
    {
        Task Execute();
        Task<List<int>> GetCelebrantsFromDateToNow(DateTime fromDate, CancellationToken token);
        Task<List<int>> GetLeadIdsWithNecessaryTransactionsCount(int transactionsCount, int daysCount, CancellationToken token);
        Task<List<int>> GetLeadsIdsWithNecessaryAmountDifference(decimal amountDifference, int daysCount, CancellationToken token);
    }
}