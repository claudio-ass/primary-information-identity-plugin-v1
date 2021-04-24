using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using PidPlugin.Cache;
using Microsoft.Extensions.Options;

namespace PidPlugin.Handlers
{
    public class PidPluginCacheHandler : DelegatingHandler
    {
        private readonly ICacheAccessor         cacheAccessor;
        private readonly IOptions<CacheOptions> options;
        
        public PidPluginCacheHandler(ICacheAccessor cacheAccessor, IOptions<CacheOptions> options)
        {
            this.cacheAccessor = cacheAccessor ??
                throw new ArgumentNullException(nameof(cacheAccessor));

            this.options = options ??
                throw new ArgumentNullException(nameof(options));
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            if (request.Method != HttpMethod.Get)
                return await base.SendAsync(request, cancellationToken);

            if (this.options.Value.Expiration.TotalMinutes <= 0)
                return await base.SendAsync(request, cancellationToken);

            string key = request.RequestUri.PathAndQuery;

            if (this.cacheAccessor.TryGetValue(key, out object value))
            {
                return new HttpResponseMessage()
                {
                    StatusCode = System.Net.HttpStatusCode.OK,
                    Content    = new StringContent(value.ToString())
                };
            }

            HttpResponseMessage response =
                await base.SendAsync(request, cancellationToken);

            string content = await response.Content.ReadAsStringAsync();

            this.cacheAccessor.Set(key, content, this.options.Value.Expiration);

            return response;
        }
    }
}
