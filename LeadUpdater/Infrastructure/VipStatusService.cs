﻿using LeadUpdater.Infrastructure;
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

    public async Task<List<int>> GetVipLeadsIds()
    {
        var vipLeadsIds = await _reportingClient.GetCelebrantsFromDateToNow(Constant.CelebrantsDaysCount, _token.Token);
        var leadsWithTransactions = await _reportingClient.GetLeadIdsWithNecessaryTransactionsCount(
            Constant.TransactionsCount,
            Constant.TrasactionDaysCount,
            _token.Token);

        var leadsWithAmount = await _reportingClient.GetLeadsIdsWithNecessaryAmountDifference(
            Constant.AmountDifference,
            Constant.AmountDifferenceDaysCount,
            _token.Token);

        vipLeadsIds.AddRange(leadsWithTransactions);
        vipLeadsIds.AddRange(leadsWithAmount);
        vipLeadsIds.Distinct();

        return vipLeadsIds;
    }
}
