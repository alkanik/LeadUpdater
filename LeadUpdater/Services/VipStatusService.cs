using IncredibleBackendContracts.Events;
using LeadUpdater.Infrastructure;
using System.Dynamic;
using System.Threading;

namespace LeadUpdater;

public class VipStatusService : IVipStatusService
{
    private readonly IReportingClient _reportingClient;
    private readonly CancellationTokenSource _token;
    private readonly ILogger<VipStatusService> _logger;

    public VipStatusService(IReportingClient reportingClient, ILogger<VipStatusService> logger)
    {
        _reportingClient = reportingClient;
        _token = new CancellationTokenSource(); ;
        _logger = logger;
    }

    public async Task<LeadsRoleUpdatedEvent> GetVipLeadsIds()
    {
        _logger.LogInformation($"Get ids leads with birthday due {Constant.CelebrantsDaysCount} days");
        var vipLeadsIds = _reportingClient.GetCelebrantsFromDateToNow(Constant.CelebrantsDaysCount, _token.Token);

        _logger.LogInformation($"Get ids leads with {Constant.TransactionsCount} transactions due {Constant.TrasactionDaysCount} days");
        var leadsWithTransactions = _reportingClient.GetLeadIdsWithNecessaryTransactionsCount(
            Constant.TransactionsCount,
            Constant.TrasactionDaysCount,
            _token.Token);

        _logger.LogInformation($"Get ids leads with amount difference {Constant.AmountDifference} due {Constant.AmountDifferenceDaysCount} days");
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