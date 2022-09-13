using IncredibleBackendContracts.Events;

namespace LeadUpdater
{
    public interface IVipStatusService
    {
        Task<LeadsRoleUpdatedEvent> GetVipLeadsIds();
    }
}