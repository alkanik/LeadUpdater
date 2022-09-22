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
        _logger.LogInformation(Constant.LogMessageGetIdsBirthday, _statusConfig.DAYS_COUNT_CELEBRANTS);
        var vipLeadsIds = _reportingClient.GetCelebrantsFromDateToNow(Int32.Parse(_statusConfig.DAYS_COUNT_CELEBRANTS), _token.Token);

        _logger.LogInformation(Constant.LogMessageGetIdsTransactions, _statusConfig.TRANSACTIONS_COUNT,_statusConfig.DAYS_COUNT_TRANSACTIONS);
        var leadsWithTransactions = _reportingClient.GetLeadIdsWithNecessaryTransactionsCount(
            Int32.Parse(_statusConfig.TRANSACTIONS_COUNT),
            Int32.Parse(_statusConfig.DAYS_COUNT_TRANSACTIONS),
            _token.Token);

        _logger.LogInformation(Constant.LogMessageGetIdsAmounts, _statusConfig.AMOUNT_DIFFERENCE, _statusConfig.DAYS_COUNT_AMOUNT);
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
        await _messageProducer.ProduceMessage(modelIds, Constant.LogMessageSentLeads);
    }

    private async Task SendMailToAdmin()
    {
        var message = new EmailEvent()
        {
            Email = _statusConfig.ADMIN_EMAIL,
            Subject = Constant.EmailSubject,
            Body = $"{DateTime.Now}{Constant.EmailBody}"
        };
        await _messageProducer.ProduceMessage<EmailEvent>(message, Constant.LogMessageSentMail);
    }

    private List<int> GetUniqueIdsList(List<int> list1, List<int> list2, List<int> list3)
    {
        list1.AddRange(list2);
        list1.AddRange(list3);
        return list1.Distinct().ToList();
    }
}