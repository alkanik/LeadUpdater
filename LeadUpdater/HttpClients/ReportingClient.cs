﻿using LeadUpdater.Policies;
using LeadUpdater.Infrastructure;
using System.Text.Json;
using Microsoft.Extensions.Options;

namespace LeadUpdater;

public class ReportingClient : IReportingClient
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly JsonSerializerOptions _options;
    private readonly ILogger<ReportingClient> _logger;
    private readonly VipStatusConfiguration _statusConfig;

    public ReportingClient(IHttpClientFactory httpClientFactory, ILogger<ReportingClient> logger, IOptions<VipStatusConfiguration> statusConfig)
    {
        _httpClientFactory = httpClientFactory;
        _options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        _logger = logger;
        _statusConfig = statusConfig.Value;
    }

    public async Task<List<int>> GetCelebrantsFromDateToNow(int daysCount, CancellationToken token)
    {
        var httpClient = _httpClientFactory.CreateClient("Reporting");

        try
        {
            using (var response = await httpClient.GetAsync($"{_statusConfig.REPORTING_BASE_ADDRESS}{Constant.LeadInfoPath}?{daysCount}",
                HttpCompletionOption.ResponseHeadersRead, token))
            {
                response.EnsureSuccessStatusCode();
                var content = await response.Content.ReadAsStreamAsync();
                var leads = await JsonSerializer.DeserializeAsync<List<int>>(content, _options);
                return leads;
            }
        }
        catch (Exception ex)
        {
            _logger.LogInformation($"{ex.Message}");
            return new List<int>();
        }
    }

    public async Task<List<int>> GetLeadIdsWithNecessaryTransactionsCount(int transactionsCount, int daysCount, CancellationToken token)
    {
        var httpClient = _httpClientFactory.CreateClient("Reporting");

        try
        {
            using (var response = await httpClient
                .GetAsync($"{_statusConfig.REPORTING_BASE_ADDRESS}{Constant.LeadStatisticsPath}{Constant.LSTransactionPath}transactionsCount={transactionsCount}&daysCount={daysCount}",
                HttpCompletionOption.ResponseHeadersRead, token))
            {
                response.EnsureSuccessStatusCode();
                var content = await response.Content.ReadAsStreamAsync();
                var leads = await JsonSerializer.DeserializeAsync<List<int>>(content, _options);
                return leads;
            }
        }
        catch (Exception ex)
        {
            _logger.LogInformation($"{ex.Message}");
            return new List<int>();
        }
    }

    public async Task<List<int>> GetLeadsIdsWithNecessaryAmountDifference(decimal amountDifference, int daysCount, CancellationToken token)
    {
        var httpClient = _httpClientFactory.CreateClient("Reporting");

        try
        {
            using (var response = await httpClient
                .GetAsync($"{_statusConfig.REPORTING_BASE_ADDRESS}{Constant.LeadStatisticsPath}{Constant.LSAmountPath}amountDifference={amountDifference}&daysCount={daysCount}",
                HttpCompletionOption.ResponseHeadersRead, token))
            {
                response.EnsureSuccessStatusCode();
                var content = await response.Content.ReadAsStreamAsync();
                var leads = await JsonSerializer.DeserializeAsync<List<int>>(content, _options);
                return leads;
            }
        }
        catch (Exception ex)
        {
            _logger.LogInformation($"{ex.Message}");
            return new List<int>();
        }
    }
}