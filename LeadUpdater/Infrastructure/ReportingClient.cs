using LeadUpdater.Policies;
using LeadUpdater.Infrastructure;
using System.Text.Json;

namespace LeadUpdater;

public class ReportingClient : IReportingClient
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly JsonSerializerOptions _options;

    public ReportingClient(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
        _options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
    }

    public async Task<List<int>> GetCelebrantsFromDateToNow(int daysCount, CancellationToken token)
    {
        var httpClient = _httpClientFactory.CreateClient("Reporting");

        try
        {
            using (var response = await httpClient.GetAsync($"{Constant.ReportingBaseAddress}{Constant.LeadInfo}?{daysCount}",
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
            using (var response = await httpClient.GetAsync($"{Constant.ReportingBaseAddress}{Constant.LeadStatistics}/{transactionsCount}/{daysCount}/transactions-count",
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
            using (var response = await httpClient.GetAsync($"{Constant.ReportingBaseAddress}{Constant.LeadStatistics}/{amountDifference}/{daysCount}/amount-difference",
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