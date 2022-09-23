﻿using IncredibleBackend.Messaging;
using IncredibleBackend.Messaging.Extentions;
using IncredibleBackend.Messaging.Interfaces;
using IncredibleBackendContracts.Constants;
using IncredibleBackendContracts.Events;
using LeadUpdater.Infrastructure;
using LeadUpdater.Policies;
using Microsoft.AspNetCore.Builder;

namespace LeadUpdater.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static void AddServices(this IServiceCollection services)
        {
            services.AddHostedService<Worker>();
            services.AddHttpClient(Constant.HttpClientName).AddPolicyHandler(
                request => request.Method == HttpMethod.Get ? new ClientPolicy().RetryPolicy : new ClientPolicy().RetryPolicy);
            services.AddScoped<IReportingClient, ReportingClient>();
            services.AddScoped<IVipStatusService, VipStatusService>();
            services.AddScoped<IScheduler, Scheduler>();
            services.AddScoped<IMessageProducer, MessageProducer>();
            services.AddSingleton<ClientPolicy>(new ClientPolicy());

            services.RegisterConsumersAndProducers(
                null,
                null,
                (cfg) =>
                {
                    cfg.RegisterProducer<LeadsRoleUpdatedEvent>(RabbitEndpoint.LeadsRoleUpdateCrm);
                    cfg.RegisterProducer<EmailEvent>(RabbitEndpoint.EmailCreate);
                });
        }
        
        public static void ConfigureService(this IServiceCollection services, WebApplicationBuilder builder)
        {
            services.Configure<HostOptions>( hostOptions =>
            {
                hostOptions.BackgroundServiceExceptionBehavior = BackgroundServiceExceptionBehavior.Ignore;
            });
        }
    }
}
