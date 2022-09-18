using LeadUpdater.Infrastructure;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;

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
        _token = new CancellationTokenSource();
        _loggerMock = new Mock<ILogger<VipStatusService>>();
        var statusConfig = Options.Create(new VipStatusConfiguration());
        _sut = new VipStatusService(
            _reportingClientMock.Object,
            _loggerMock.Object,
            statusConfig);
    }
    
    [Fact]
    public void Test1()
    {

    }
}