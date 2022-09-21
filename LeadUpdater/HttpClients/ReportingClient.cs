﻿using LeadUpdater.Policies;
using LeadUpdater.Infrastructure;
using System.Text.Json;
using Microsoft.Extensions.Options;
using IncredibleBackend.Messaging.Interfaces;
using IncredibleBackendContracts.Events;

namespace LeadUpdater;

public class ReportingClient : IReportingClient
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly JsonSerializerOptions _options;
    private readonly ILogger<ReportingClient> _logger;
    private readonly VipStatusConfiguration _statusConfig;
    private readonly IMessageProducer _messageProducer;

    public ReportingClient(IHttpClientFactory httpClientFactory, ILogger<ReportingClient> logger, IOptions<VipStatusConfiguration> statusConfig, IMessageProducer messageProducer)
    {
        _httpClientFactory = httpClientFactory;
        _options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        _logger = logger;
        _statusConfig = statusConfig.Value;
        _messageProducer = messageProducer;
    }

    public async Task<List<int>> GetCelebrantsFromDateToNow(int daysCount, CancellationToken token)
    {
        var httpClient = _httpClientFactory.CreateClient("Reporting");

        try
        {
            using (var response = await httpClient.GetAsync($"{_statusConfig.REPORTING_BASE_ADDRESS}{Constant.LeadInfoPath}{daysCount}",
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
            var message = new EmailEvent() 
            { 
                Email = _statusConfig.ADMIN_EMAIL, 
                Subject = "Lead Updater couldn't receive leads from Reporting", 
                Body = $"{ex.Message}\n {DateTime.Now}" 
            };

            _messageProducer.ProduceMessage<EmailEvent>(message, $"{ex.Message}");
            throw;
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