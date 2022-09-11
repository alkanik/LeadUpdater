using LeadUpdater.Policies;
using System.Text.Json;

namespace LeadUpdater;

public class ReportingClient : IReportingClient
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly JsonSerializerOptions _options;
    private readonly CancellationTokenSource _cancellationTokenSource;
    private readonly ClientPolicy _policy;

    public ReportingClient(IHttpClientFactory httpClientFactory, ClientPolicy clientPolicy)
    {
        _httpClientFactory = httpClientFactory;
        _options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        _cancellationTokenSource = new CancellationTokenSource();
        _policy = clientPolicy;
    }

    public async Task Execute()
    {
        await GetCelebrantsFromDateToNow(new DateTime(2022, 7, 9), _cancellationTokenSource.Token);
        await GetLeadIdsWithNecessaryTransactionsCount(42, 60, _cancellationTokenSource.Token);
        await GetLeadsIdsWithNecessaryAmountDifference(13000, 30, _cancellationTokenSource.Token);
    }

    public async Task<List<int>> GetCelebrantsFromDateToNow(DateTime fromDate, CancellationToken token)
    {
        var httpClient = _httpClientFactory.CreateClient("Reporting");

        try
        {
            using (var response = await httpClient.GetAsync($"https://piter-education.ru:6010/LeadInfo?fromDate={fromDate.ToString("dd.MM.yyyy")}",
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
            Console.WriteLine(ex.Message);
            return new List<int>();
        }
    }

    public async Task<List<int>> GetLeadIdsWithNecessaryTransactionsCount(int transactionsCount, int daysCount, CancellationToken token)
    {
        var httpClient = _httpClientFactory.CreateClient("Reporting");

        try
        {
            using (var response = await httpClient.GetAsync($"https://piter-education.ru:6010/LeadStatistics/{transactionsCount}/{daysCount}/transactions-count",
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
            Console.WriteLine(ex.Message);
            return new List<int>();
        }
    }

    public async Task<List<int>> GetLeadsIdsWithNecessaryAmountDifference(decimal amountDifference, int daysCount, CancellationToken token)
    {
        var httpClient = _httpClientFactory.CreateClient("Reporting");

        try
        {
            using (var response = await httpClient.GetAsync($"https://piter-education.ru:6010/LeadStatistics/{amountDifference}/{daysCount}/amount-difference",
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
            Console.WriteLine(ex.Message);
            return new List<int>();
        }
    }
}