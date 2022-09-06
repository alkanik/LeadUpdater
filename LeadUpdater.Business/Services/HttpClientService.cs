using System.Text.Json;
using System.Threading.Tasks;

namespace LeadUpdater.Business;

public class HttpClientService : IHttpClientService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly JsonSerializerOptions _options;
    private readonly CancellationTokenSource _cancellationTokenSource;


    public HttpClientService(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
        _options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        _cancellationTokenSource = new CancellationTokenSource();
    }

    public async Task Execute()
    {
        await GetServiceFromYogurtCleaningForTest(_cancellationTokenSource.Token);
        //await GetCelebrantsFromDateToNow(new DateTime(2022, 7, 21), _cancellationTokenSource.Token);
        //await GetLeadIdsWithNecessaryTransactionsCount(42, _cancellationTokenSource.Token);
        //await GetLeadsIdsWithNecessaryAmountDifference(13000, _cancellationTokenSource.Token);
        //_cancellationTokenSource.CancelAfter(20000);
    }


    public async Task GetServiceFromYogurtCleaningForTest(CancellationToken token) 
    {
        var httpClient = _httpClientFactory.CreateClient();
        try
        {
            using (var response = await httpClient.GetAsync("https://piter-education.ru:10042/Services/275", HttpCompletionOption.ResponseHeadersRead, token))
            {
                response.EnsureSuccessStatusCode();
                var stream = await response.Content.ReadAsStreamAsync();
                var service = JsonSerializer.DeserializeAsync<string>(stream, _options);
            }
        }
        catch(OperationCanceledException ocex)
        {
            var exceptionMessage = ocex.Message; // logging
        }
    }

    public async Task GetCelebrantsFromDateToNow(DateTime fromDate, CancellationToken token)
    {
        var httpClient = _httpClientFactory.CreateClient();
        try
        {
            using (var response = await httpClient.GetAsync("LeadInfo", HttpCompletionOption.ResponseHeadersRead, token))
            {
                response.EnsureSuccessStatusCode();
                var stream = await response.Content.ReadAsStreamAsync();
                var celebrants = JsonSerializer.DeserializeAsync<List<int>>(stream, _options);
            }
        }
        catch (OperationCanceledException ocex)
        {
            var exceptionMessage = ocex.Message;
        }
    }

    public async Task GetLeadIdsWithNecessaryTransactionsCount(int transactionsCount, int daysCount, CancellationToken token)
    {
        var httpClient = _httpClientFactory.CreateClient();
        try
        {
            using (var response = await httpClient.GetAsync("LeadStatistics/transactionsCount", HttpCompletionOption.ResponseHeadersRead, token))
            {
                response.EnsureSuccessStatusCode();
                var stream = await response.Content.ReadAsStreamAsync();
                var leads = JsonSerializer.DeserializeAsync<List<int>>(stream, _options);
            }
        }
        catch (OperationCanceledException ocex)
        {
            var exceptionMessage = ocex.Message;
        }
    }

    public async Task GetLeadsIdsWithNecessaryAmountDifference(decimal amountDifference, int daysCount, CancellationToken token)
    {
        var httpClient = _httpClientFactory.CreateClient();
        try
        {
            using (var response = await httpClient.GetAsync("LeadStatistics", HttpCompletionOption.ResponseHeadersRead, token))
            {
                response.EnsureSuccessStatusCode();
                var stream = await response.Content.ReadAsStreamAsync();
                var leads = JsonSerializer.DeserializeAsync<List<int>>(stream, _options);
            }
        }
        catch (OperationCanceledException ocex)
        {
            var exceptionMessage = ocex.Message;
        }
    }
}