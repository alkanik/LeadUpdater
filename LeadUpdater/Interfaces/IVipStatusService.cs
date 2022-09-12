namespace LeadUpdater
{
    public interface IVipStatusService
    {
        Task<List<int>> GetVipLeadsIds();
    }
}