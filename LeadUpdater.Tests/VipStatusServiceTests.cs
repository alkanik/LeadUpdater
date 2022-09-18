using LeadUpdater.Infrastructure;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using System.Security.Cryptography;

namespace LeadUpdater.Tests;

public class VipStatusServiceTests
{
    private VipStatusService _sut;
    private Mock<IReportingClient> _reportingClientMock;
    private CancellationTokenSource _token;
    private Mock<ILogger<VipStatusService>> _loggerMock;
    private VipStatusConfiguration _statusConfig;

    public VipStatusServiceTests()
    {
        _reportingClientMock = new Mock<IReportingClient>();
        _loggerMock = new Mock<ILogger<VipStatusService>>();
        var statusConfig = Options.Create(new VipStatusConfiguration()
        {
            AMOUNT_DIFFERENCE = "1",
            TRANSACTIONS_COUNT = "1",
            DAYS_COUNT_AMOUNT = "1",
            DAYS_COUNT_CELEBRANTS = "1",
            DAYS_COUNT_TRANSACTIONS = "1"
        });
        _sut = new VipStatusService(
            _reportingClientMock.Object,
            _loggerMock.Object,
            statusConfig);
    }
    
    [Fact]
    public async Task GetVipLeadsIdsTest_WhenIdsExist_ThenItsGetted()
    {
        // given
        var idsCeleb = new List<int> { 1, 2, 3 };
        var idsTr = new List<int> { 4,5,6 };
        var idsAm = new List<int> { 77,98,11 };
        var token = new CancellationTokenSource();

        _reportingClientMock.Setup(c => c.GetCelebrantsFromDateToNow(1, It.IsAny<CancellationToken>())).ReturnsAsync(idsCeleb);
        _reportingClientMock.Setup(c => c.GetLeadIdsWithNecessaryTransactionsCount(1, 1, It.IsAny<CancellationToken>())).ReturnsAsync(idsTr);
        _reportingClientMock.Setup(c => c.GetLeadsIdsWithNecessaryAmountDifference(1, 1, It.IsAny<CancellationToken>())).ReturnsAsync(idsAm);

        var expectedCount = 9;

        // when
        var actual = await _sut.GetVipLeadsIds();

        // then
        Assert.Equal(expectedCount, actual.Ids.Count);
        _reportingClientMock.Verify(c => c.GetCelebrantsFromDateToNow(It.IsAny<int>(), It.IsAny<CancellationToken>()), Times.Once);
        _reportingClientMock.Verify(c => c.GetLeadIdsWithNecessaryTransactionsCount(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<CancellationToken>()), Times.Once);
        _reportingClientMock.Verify(c => c.GetLeadsIdsWithNecessaryAmountDifference(It.IsAny<decimal>(), It.IsAny<int>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GetVipLeadsIdsTest_WhenIdsDoNotExist_ThenItsNotGetted()
    {
        // given
        var emptyList = new List<int>();

        _reportingClientMock.Setup(c => c.GetCelebrantsFromDateToNow(1, It.IsAny<CancellationToken>())).ReturnsAsync(emptyList);
        _reportingClientMock.Setup(c => c.GetLeadIdsWithNecessaryTransactionsCount(1, 1, It.IsAny<CancellationToken>())).ReturnsAsync(emptyList);
        _reportingClientMock.Setup(c => c.GetLeadsIdsWithNecessaryAmountDifference(1, 1, It.IsAny<CancellationToken>())).ReturnsAsync(emptyList);

        var expectedCount = 0;

        // when
        var actual = await _sut.GetVipLeadsIds();

        // then
        Assert.Equal(expectedCount, actual.Ids.Count);
        _reportingClientMock.Verify(c => c.GetCelebrantsFromDateToNow(It.IsAny<int>(), It.IsAny<CancellationToken>()), Times.Once);
        _reportingClientMock.Verify(c => c.GetLeadIdsWithNecessaryTransactionsCount(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<CancellationToken>()), Times.Once);
        _reportingClientMock.Verify(c => c.GetLeadsIdsWithNecessaryAmountDifference(It.IsAny<decimal>(), It.IsAny<int>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GetVipLeadsIdsTest_WhenTheSameIdsGettedFromReporting_ThenListOfUniqueIdsGetted()
    {
        // given
        var idsCeleb = new List<int> { 1, 2, 3, 4 };
        var idsTr = new List<int> { 1,2 };
        var idsAm = new List<int> { 4,5 };
        var token = new CancellationTokenSource();

        _reportingClientMock.Setup(c => c.GetCelebrantsFromDateToNow(1, It.IsAny<CancellationToken>())).ReturnsAsync(idsCeleb);
        _reportingClientMock.Setup(c => c.GetLeadIdsWithNecessaryTransactionsCount(1, 1, It.IsAny<CancellationToken>())).ReturnsAsync(idsTr);
        _reportingClientMock.Setup(c => c.GetLeadsIdsWithNecessaryAmountDifference(1, 1, It.IsAny<CancellationToken>())).ReturnsAsync(idsAm);

        var expectedCount = 5;

        // when
        var actual = await _sut.GetVipLeadsIds();

        // then
        Assert.Equal(expectedCount, actual.Ids.Count);
        _reportingClientMock.Verify(c => c.GetCelebrantsFromDateToNow(It.IsAny<int>(), It.IsAny<CancellationToken>()), Times.Once);
        _reportingClientMock.Verify(c => c.GetLeadIdsWithNecessaryTransactionsCount(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<CancellationToken>()), Times.Once);
        _reportingClientMock.Verify(c => c.GetLeadsIdsWithNecessaryAmountDifference(It.IsAny<decimal>(), It.IsAny<int>(), It.IsAny<CancellationToken>()), Times.Once);
    }
}