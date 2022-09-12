﻿namespace LeadUpdater.Infrastructure;

public class Constant
{
    public const string ReportingBaseAddress = "https://piter-education.ru:6010/";
    public const string LeadInfo = "LeadInfo";
    public const string LeadStatistics = "LeadStatistics";
    public const int CelebrantsDaysCount = 14;
    public const int TransactionsCount = 42;
    public const int TrasactionDaysCount = 60;
    public const decimal AmountDifference = 13000;
    public const int AmountDifferenceDaysCount = 30;
    public const int TimeDelay = 5000;
    public const string CronExpressionTest = "0 * * ? * *";
    public const string CronExpression = "0 0 23 ? * *";
}