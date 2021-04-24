using System;
using PidPlugin.Settings;
using PidPlugin.Handlers;
using PidPlugin.Cache;
using Microsoft.Extensions.DependencyInjection;

namespace PidPlugin.Extensions
{
    public static class ConnectorExtensions
    {
        private const string SubscriptionKeyHeader = "Ocp-Apim-Subscription-Key";

        public static IServiceCollection AddPidConnector(this IServiceCollection services, PidPluginSettings pidConnectorSettings)
        {
            services.Configure<CacheOptions>(options => options.Expiration 
                = TimeSpan.FromMinutes(pidConnectorSettings.CacheExpirationInMinutes));

            services.AddMemoryCache();

            services.AddSingleton<ICacheAccessor, MemoryCacheAccessor>();

            services.AddHttpClient();

            services.AddTransient<PidPluginExceptionHandler>();
            services.AddTransient<PidPluginCacheHandler>();

            services.AddHttpClient<IPidPluginClient, PidPluginClient>()
                .ConfigureHttpClient(builder =>
                {
                    builder.BaseAddress = 
                        new Uri(pidConnectorSettings.BaseUrl);

                    builder.Timeout = 
                        TimeSpan.FromMinutes(pidConnectorSettings.TimeoutInMinutes);

                    builder.DefaultRequestHeaders.Add(
                        SubscriptionKeyHeader, 
                        pidConnectorSettings.SubscriptionKey
                    );
                })
                .AddHttpMessageHandler<PidPluginCacheHandler>()
                .AddHttpMessageHandler<PidPluginExceptionHandler>();

            return services;
        }
    }
}
