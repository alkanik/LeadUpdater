using IncredibleBackend.Messaging.Interfaces;
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
    private readonly IMessageProducer _messageProducer;

    public VipStatusService(
        IReportingClient reportingClient, 
        ILogger<VipStatusService> logger, 
        IOptions<VipStatusConfiguration> statusConfig, 
        IMessageProducer messageProducer)
    {
        _reportingClient = reportingClient;
        _token = new CancellationTokenSource();
        _logger = logger;
        _statusConfig = statusConfig.Value;
        _messageProducer = messageProducer;
    }

    public async Task GetVipLeadsIds()
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

        await Task.WhenAll(vipLeadsIds, leadsWithTransactions, leadsWithAmount);
        
        if (vipLeadsIds.Result == null || leadsWithTransactions.Result == null || leadsWithAmount.Result == null)
        {
            await SendMailToAdmin();
        }
        else
        {
            var vipIds = GetUniqueIdsList(vipLeadsIds.Result, leadsWithTransactions.Result, leadsWithAmount.Result);
            await SendLeadsIdsToQueue(vipIds);
        }
    }

    private async Task SendLeadsIdsToQueue(List<int> ids)
    {
        var modelIds = new LeadsRoleUpdatedEvent(ids);
        await _messageProducer.ProduceMessage(modelIds, "Sent Lead's Ids to Queue");
    }

    private async Task SendMailToAdmin()
    {
        var message = new EmailEvent()
        {
            Email = _statusConfig.ADMIN_EMAIL,
            Subject = "Lead Updater couldn't receive leads from Reporting",
            Body = $"{DateTime.Now} Some http request to reporting returned null. Go to see logs."
        };
        await _messageProducer.ProduceMessage<EmailEvent>(message, "Sent mail for Admin to Queue");
    }

    private List<int> GetUniqueIdsList(List<int> list1, List<int> list2, List<int> list3)
    {
        list1.AddRange(list2);
        list1.AddRange(list3);
        return list1.Distinct().ToList();
    }
}