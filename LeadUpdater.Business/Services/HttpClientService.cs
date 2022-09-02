using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace LeadUpdater.Business;

public class HttpClientService : IHttpClientService
{
    private static readonly HttpClient _httpClient = new HttpClient();
    private readonly JsonSerializerOptions _options;
    private readonly ILogger<HttpClientService> _logger;

    public HttpClientService(ILogger<HttpClientService> logger)
    {
        _httpClient.BaseAddress = new Uri("https://piter-education.ru:10042/api/");
        _httpClient.Timeout = new TimeSpan(0, 0, 30);
        _httpClient.DefaultRequestHeaders.Clear();
        _logger = logger;
        _options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
    }

    public async Task Execute()
    {
        await GetServiceFromYogurtCleaningForTest(275);
        //await GetCelebrantsFromDateToNow(new DateTime(2022, 7, 21));
        //await GetLeadIdsWithNecessaryTransactionsCount(42);
        //await GetLeadsIdsWithNecessaryAmountDifference(13000);
    }
    
    public async Task<string> GetServiceFromYogurtCleaningForTest(int id) 
    {
        var response = await _httpClient.GetAsync("Services/{id}");
        response.EnsureSuccessStatusCode();
        var stream = await response.Content.ReadAsStreamAsync();
        var service = JsonSerializer.Deserialize<string>(stream, _options);
        return service;
    }
    public async Task<List<int>> GetCelebrantsFromDateToNow(DateTime date)
    {
        using (var response = await _httpClient.GetAsync("LeadInfo", HttpCompletionOption.ResponseHeadersRead))
        {
            response.EnsureSuccessStatusCode();
            var stream = await response.Content.ReadAsStreamAsync();
            var celebrants = JsonSerializer.Deserialize<List<int>>(stream, _options);
            return celebrants;
            _logger.LogInformation($"Received {celebrants.Count} celebrants from {date} to {DateTime.Now}");
        }
    }

    public async Task<List<int>> GetLeadIdsWithNecessaryTransactionsCount(int count)
    {
        var response = await _httpClient.GetAsync("LeadStatistics/transactionsCount");
        response.EnsureSuccessStatusCode();
        var stream = await response.Content.ReadAsStreamAsync();
        var leads = JsonSerializer.Deserialize<List<int>>(stream, _options);
        _logger.LogInformation($"Received {leads.Count} leads with {count} transactions at {DateTime.Now}");
        return leads;
    }

    public async Task<List<int>> GetLeadsIdsWithNecessaryAmountDifference(double amount)
    {
        var response = await _httpClient.GetAsync("LeadStatistics");
        response.EnsureSuccessStatusCode();
        var stream = await response.Content.ReadAsStreamAsync();
        var leads = JsonSerializer.Deserialize<List<int>>(stream, _options);
        _logger.LogInformation($"Received {leads.Count} leads with {amount} difference at {DateTime.Now}");
        return leads;
    }
}