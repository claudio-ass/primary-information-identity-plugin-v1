using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using PidPlugin.Dtos;
using Newtonsoft.Json;

namespace PidPlugin
{
    public class PidPluginClient : IPidPluginClient
    {
        protected readonly HttpClient httpClient = null;

        public PidPluginClient(HttpClient httpClient)
        {
            this.httpClient = httpClient ??
                throw new ArgumentNullException(nameof(httpClient));
        }

        public async Task<EntityBasicData> GetEntityDataBasicAsync(string cuit, CancellationToken cancellationToken = default)
        {
            string url = $"EntityDataBasic?key={cuit}";

            return await GetAsync<EntityBasicData>(url);
        }

        public async Task<EntityFullData> GetEntityDataFullAsync(string cuit, CancellationToken cancellationToken = default)
        {
            string url = $"EntityDataFull?key={cuit}";

            return await GetAsync<EntityFullData>(url);
        }

        public async Task<SpecialRecordEntry> GetSpecialRecordsAsync(string cuit, string rule, CancellationToken cancellationToken = default)
        {
            string url = $"SpecialRecord?key={cuit}&rule={rule}";

            return await GetAsync<SpecialRecordEntry>(url);
        }

        public async Task<BankAccountDetail> GetBankAccountDetailAsync(string cbu, CancellationToken cancellationToken = default)
        {
            string url = $"BankAccountDetails?key={cbu}";

            return await GetAsync<BankAccountDetail>(url);
        }

        public async Task<BankAccountOwner> GetBankAccountOwnershipAsync(string cbu, string cuit, CancellationToken cancellationToken = default)
        {
            string url = $"BankAccountOwnership?account_address={cbu}&owner_key={cuit}";

            return await GetAsync<BankAccountOwner>(url);
        }

        private async Task<TResponse> GetAsync<TResponse>(string url, CancellationToken cancellationToken = default)
        {
            HttpResponseMessage httpResponseMessage =
                await this.httpClient.GetAsync(url, cancellationToken);

            string content = await httpResponseMessage.Content
                .ReadAsStringAsync();

            return JsonConvert.DeserializeObject<TResponse>(content);
        }
    }
}
