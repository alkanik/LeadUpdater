using IncredibleBackendContracts.Events;
using LeadUpdater.Infrastructure;
using System.Dynamic;
using System.Threading;

namespace LeadUpdater;

public class VipStatusService : IVipStatusService
{
    private readonly IReportingClient _reportingClient;
    private readonly CancellationTokenSource _token;

    public VipStatusService(IReportingClient reportingClient)
    {
        _reportingClient = reportingClient;
        _token = new CancellationTokenSource(); ;
    }

    public async Task<LeadsRoleUpdatedEvent> GetVipLeadsIds()
    {
        var vipLeadsIds = _reportingClient.GetCelebrantsFromDateToNow(Constant.CelebrantsDaysCount, _token.Token);
        var leadsWithTransactions = _reportingClient.GetLeadIdsWithNecessaryTransactionsCount(
            Constant.TransactionsCount,
            Constant.TrasactionDaysCount,
            _token.Token);

        var leadsWithAmount = _reportingClient.GetLeadsIdsWithNecessaryAmountDifference(
            Constant.AmountDifference,
            Constant.AmountDifferenceDaysCount,
            _token.Token);

        await Task.WhenAll(vipLeadsIds, leadsWithTransactions, leadsWithTransactions);

        vipLeadsIds.Result.AddRange(leadsWithTransactions.Result);
        vipLeadsIds.Result.AddRange(leadsWithAmount.Result);
        vipLeadsIds.Result.Distinct();

        return new LeadsRoleUpdatedEvent(vipLeadsIds.Result);
    }
}