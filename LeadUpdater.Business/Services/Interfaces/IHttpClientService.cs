namespace LeadUpdater.Business
{
    public interface IHttpClientService
    {
        Task Execute();
        Task<List<int>> GetCelebrantsFromDateToNow(DateTime date);
        Task<List<int>> GetLeadIdsWithNecessaryTransactionsCount(int count);
        Task<List<int>> GetLeadsIdsWithNecessaryAmountDifference(double amount);
    }
}