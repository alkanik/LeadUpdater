using IncredibleBackend.Messaging.Interfaces;
using IncredibleBackendContracts.Events;
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
    private Mock<ILogger<VipStatusService>> _loggerMock;
    private VipStatusConfiguration _statusConfig;
    private Mock<IMessageProducer> _producerMock;

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
        _producerMock = new Mock<IMessageProducer>();
        _sut = new VipStatusService(
            _reportingClientMock.Object,
            _loggerMock.Object,
            statusConfig, 
            _producerMock.Object);
    }
    
    [Fact]
    public async Task GetVipLeadsIdsTest_WhenRequestsPassed_ThenIdsSentToQueue()
    {
        // given
        var idsCeleb = new List<int> { 1, 2, 3 };
        var idsTr = new List<int> { 4,5,6 };
        var idsAm = new List<int> { 77,98,11 };

        _reportingClientMock.Setup(c => c.GetCelebrantsFromDateToNow(1, It.IsAny<CancellationToken>())).ReturnsAsync(idsCeleb);
        _reportingClientMock.Setup(c => c.GetLeadIdsWithNecessaryTransactionsCount(1, 1, It.IsAny<CancellationToken>())).ReturnsAsync(idsTr);
        _reportingClientMock.Setup(c => c.GetLeadsIdsWithNecessaryAmountDifference(1, 1, It.IsAny<CancellationToken>())).ReturnsAsync(idsAm);

        var expectedCount = 9;

        // when
        await _sut.GetVipLeadsIds();

        // then
        _reportingClientMock.Verify(c => c.GetCelebrantsFromDateToNow(1, It.IsAny<CancellationToken>()), Times.AtLeastOnce);
        _reportingClientMock.Verify(c => c.GetLeadIdsWithNecessaryTransactionsCount(1, 1, It.IsAny<CancellationToken>()), Times.AtLeastOnce);
        _reportingClientMock.Verify(c => c.GetLeadsIdsWithNecessaryAmountDifference(1, 1, It.IsAny<CancellationToken>()), Times.AtLeastOnce);
        _producerMock.Verify(p => p.ProduceMessage(It.Is<LeadsRoleUpdatedEvent>(i => i.Ids.Count == expectedCount), It.IsAny<string>()),Times.Once);
        _producerMock.Verify(p => p.ProduceMessage(It.IsAny<EmailEvent>(), It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task GetVipLeadsIdsTest_WhenIdsDoNotExist_ThenEmptyListSentToQueue()
    {
        // given
        var emptyList = new List<int>();

        _reportingClientMock.Setup(c => c.GetCelebrantsFromDateToNow(1, It.IsAny<CancellationToken>())).ReturnsAsync(emptyList);
        _reportingClientMock.Setup(c => c.GetLeadIdsWithNecessaryTransactionsCount(1, 1, It.IsAny<CancellationToken>())).ReturnsAsync(emptyList);
        _reportingClientMock.Setup(c => c.GetLeadsIdsWithNecessaryAmountDifference(1, 1, It.IsAny<CancellationToken>())).ReturnsAsync(emptyList);

        var expectedCount = 0;

        // when
        await _sut.GetVipLeadsIds();

        // then
        _reportingClientMock.Verify(c => c.GetCelebrantsFromDateToNow(1, It.IsAny<CancellationToken>()), Times.AtLeastOnce);
        _reportingClientMock.Verify(c => c.GetLeadIdsWithNecessaryTransactionsCount(1, 1, It.IsAny<CancellationToken>()), Times.AtLeastOnce);
        _reportingClientMock.Verify(c => c.GetLeadsIdsWithNecessaryAmountDifference(1, 1, It.IsAny<CancellationToken>()), Times.AtLeastOnce);
        _producerMock.Verify(p => p.ProduceMessage(It.Is<LeadsRoleUpdatedEvent>(i => i.Ids.Count == expectedCount), It.IsAny<string>()), Times.Once);
        _producerMock.Verify(p => p.ProduceMessage(It.IsAny<EmailEvent>(), It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task GetVipLeadsIdsTest_WhenTheSameIdsGettedFromReporting_ThenListOfUniqueIdsSent()
    {
        // given
        var idsCeleb = new List<int> { 1, 2, 3, 4 };
        var idsTr = new List<int> { 1,2 };
        var idsAm = new List<int> { 4,5 };

        _reportingClientMock.Setup(c => c.GetCelebrantsFromDateToNow(1, It.IsAny<CancellationToken>())).ReturnsAsync(idsCeleb);
        _reportingClientMock.Setup(c => c.GetLeadIdsWithNecessaryTransactionsCount(1, 1, It.IsAny<CancellationToken>())).ReturnsAsync(idsTr);
        _reportingClientMock.Setup(c => c.GetLeadsIdsWithNecessaryAmountDifference(1, 1, It.IsAny<CancellationToken>())).ReturnsAsync(idsAm);

        var expectedCount = 5;

        // when
        await _sut.GetVipLeadsIds();

        // then
        _reportingClientMock.Verify(c => c.GetCelebrantsFromDateToNow(1, It.IsAny<CancellationToken>()), Times.AtLeastOnce);
        _reportingClientMock.Verify(c => c.GetLeadIdsWithNecessaryTransactionsCount(1, 1, It.IsAny<CancellationToken>()), Times.AtLeastOnce);
        _reportingClientMock.Verify(c => c.GetLeadsIdsWithNecessaryAmountDifference(1, 1, It.IsAny<CancellationToken>()), Times.AtLeastOnce);
        _producerMock.Verify(p => p.ProduceMessage(It.Is<LeadsRoleUpdatedEvent>(i => i.Ids.Count == expectedCount), It.IsAny<string>()), Times.Once);
        _producerMock.Verify(p => p.ProduceMessage(It.IsAny<EmailEvent>(), It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task GetVipLeadsIdsTest_WhenAnyRequestGetNull_ThenMailToAdminSent()
    {
        // given
        var ids = new List<int> { 1, 2, 3, 4 };
        List<int> ids2 = null;

        _reportingClientMock.Setup(c => c.GetCelebrantsFromDateToNow(1, It.IsAny<CancellationToken>())).ReturnsAsync(ids);
        _reportingClientMock.Setup(c => c.GetLeadIdsWithNecessaryTransactionsCount(1, 1, It.IsAny<CancellationToken>())).ReturnsAsync(ids);
        _reportingClientMock.Setup(c => c.GetLeadsIdsWithNecessaryAmountDifference(1, 1, It.IsAny<CancellationToken>())).ReturnsAsync(ids2);

        // when
        await _sut.GetVipLeadsIds();

        // then
        _reportingClientMock.Verify(c => c.GetCelebrantsFromDateToNow(1, It.IsAny<CancellationToken>()), Times.AtLeastOnce);
        _reportingClientMock.Verify(c => c.GetLeadIdsWithNecessaryTransactionsCount(1, 1, It.IsAny<CancellationToken>()), Times.AtLeastOnce);
        _reportingClientMock.Verify(c => c.GetLeadsIdsWithNecessaryAmountDifference(1, 1, It.IsAny<CancellationToken>()), Times.AtLeastOnce);
        _producerMock.Verify(p => p.ProduceMessage(It.IsAny<LeadsRoleUpdatedEvent>(), It.IsAny<string>()), Times.Never);
        _producerMock.Verify(p => p.ProduceMessage(It.IsAny<EmailEvent>(), It.IsAny<string>()), Times.Once);
    }
}