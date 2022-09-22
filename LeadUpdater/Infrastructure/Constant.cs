namespace LeadUpdater.Infrastructure;

public class Constant
{
    public const string ServiceName = "LeadUpdater";
    public const string LeadInfoPath = "LeadInfo?";
    public const string LeadStatisticsTransactionPath = "LeadStatistics/transactions-count?";
    public const string LeadStatisticsAmountPath = "LeadStatistics/amount-difference?";
    public const string CronExpressionTest = "0 * * ? * *";
    public const string CronExpression = "0 0 * ? * *";
    public const string LogMessageLeadUpdaterRun = "LeadUpdater running at: ";
    public const string LogMessageNextUpdate = "Next update will be at: ";
    public const string LogMessageGetIdsBirthday = "Get ids leads with birthday due {0} days";
    public const string LogMessageGetIdsTransactions = "Get ids leads with {0} transactions due {1} days";
    public const string LogMessageGetIdsAmounts = "Get ids leads with amount difference {0} due {1} days";
    public const string LogMessageSentLeads = "Sent Lead's Ids to Queue";
    public const string LogMessageSentMail = "Sent mail for Admin to Queue";
    public const string EmailSubject = "LeadUpdater couldn't receive leads from Reporting";
    public const string EmailBody = "Some http request to reporting returned null. Go to see logs.";
    public const string HttpClientName = "Reporting";
}