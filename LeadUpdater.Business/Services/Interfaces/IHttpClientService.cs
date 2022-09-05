namespace LeadUpdater.Business
{
    public interface IHttpClientService
    {
        Task Execute();
        Task GetServiceFromYogurtCleaningForTest(CancellationToken token);
        Task GetCelebrantsFromDateToNow(DateTime fromDate, CancellationToken token);
        Task GetLeadIdsWithNecessaryTransactionsCount(int transactionsCount, int daysCount, CancellationToken token);
        Task GetLeadsIdsWithNecessaryAmountDifference(decimal amountDifference, int daysCount, CancellationToken token);
    }
}