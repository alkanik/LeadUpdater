using IncredibleBackendContracts.Events;
using LeadUpdater.Infrastructure;
using Microsoft.Extensions.Options;
using System.Dynamic;
using System.Threading;

namespace LeadUpdater;

public class VipStatusService : IVipStatusService
{
    private readonly IReportingClient _reportingClient;
    private readonly CancellationTokenSource _token;
    private readonly ILogger<VipStatusService> _logger;
    private readonly VipStatusConfiguration _statusConfig;

    public VipStatusService(IReportingClient reportingClient, ILogger<VipStatusService> logger, IOptions<VipStatusConfiguration> statusConfig)
    {
        _reportingClient = reportingClient;
        _token = new CancellationTokenSource();
        _logger = logger;
        _statusConfig = statusConfig.Value;
    }

    public async Task<LeadsRoleUpdatedEvent> GetVipLeadsIds()
    {
        _logger.LogInformation($"Get ids leads with birthday due {_statusConfig.DAYS_COUNT_CELEBRANTS} days");
        var vipLeadsIds = _reportingClient.GetCelebrantsFromDateToNow(Int32.Parse(_statusConfig.DAYS_COUNT_CELEBRANTS), _token.Token);

        _logger.LogInformation($"Get ids leads with {_statusConfig.TRANSACTIONS_COUNT} transactions due {_statusConfig.DAYS_COUNT_TRANSACTIONS} days");
        var leadsWithTransactions = _reportingClient.GetLeadIdsWithNecessaryTransactionsCount(
            Int32.Parse(_statusConfig.TRANSACTIONS_COUNT),
            Int32.Parse(_statusConfig.DAYS_COUNT_TRANSACTIONS),
            _token.Token);

        _logger.LogInformation($"Get ids leads with amount difference {_statusConfig.AMOUNT_DIFFERENCE} due {_statusConfig.DAYS_COUNT_AMOUNT} days");
        var leadsWithAmount = _reportingClient.GetLeadsIdsWithNecessaryAmountDifference(
            Decimal.Parse(_statusConfig.AMOUNT_DIFFERENCE),
            Int32.Parse(_statusConfig.DAYS_COUNT_AMOUNT),
            _token.Token);

        await Task.WhenAll(vipLeadsIds, leadsWithTransactions, leadsWithTransactions);

        vipLeadsIds.Result.AddRange(leadsWithTransactions.Result);
        vipLeadsIds.Result.AddRange(leadsWithAmount.Result);
        vipLeadsIds.Result.Distinct();

        return new LeadsRoleUpdatedEvent(vipLeadsIds.Result);
    }
}