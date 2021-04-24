using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using PidPlugin;
using PidPlugin.Dtos;
using PidPlugin.Extensions;
using PidPlugin.Settings;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;

namespace PidPluginUseSample
{
    class Program
    {
        static async Task Main(string[] args)
        {
            ServiceCollection services = new ServiceCollection();

            IConfiguration configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build();

            AddPidPlugin(services, configuration);

            ServiceProvider serviceProvider =
                services.BuildServiceProvider();

            var pidPluginClient = serviceProvider.GetService<IPidPluginClient>();

            try
            {
                string cuit = "cuit-number-here";

                CancellationToken cancellationToken = CancellationToken.None;

                EntityFullData entityFullDataResponse = await pidPluginClient
                    .GetEntityDataFullAsync(cuit, cancellationToken);

                Console.WriteLine(JsonConvert.SerializeObject(entityFullDataResponse, Formatting.Indented));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[{ex.GetType().Name}] {ex.Message}");
            }
        }

        static void AddPidPlugin(IServiceCollection services, IConfiguration configuration)
        {
            PidPluginSettings pidConnectorSettings = 
                new PidPluginSettings();

            configuration.GetSection("PidPluginSettings")
                .Bind(pidConnectorSettings);

            services.AddPidConnector(pidConnectorSettings);
        }
    }
}
