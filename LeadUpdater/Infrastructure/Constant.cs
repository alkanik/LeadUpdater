namespace LeadUpdater.Infrastructure;

public class Constant
{
    public const string LeadInfoPath = "LeadInfo";
    public const string LeadStatisticsPath = "LeadStatistics";
    public const string LSTransactionPath = "/transactions-count?";
    public const string LSAmountPath = "/amount-difference?";
    public const int TimeDelay = 5000;
    public const string CronExpressionTest = "0 * * ? * *";
    public const string CronExpression = "0 0 23 ? * *"; //0 0 * ? * * *
}