using Microsoft.Extensions.DependencyInjection;
using System;

namespace TeamMonitoring.Common.HttpClient
{
    public static class HttpClientExtensions
    {
        public static IServiceCollection AddHttpClientService(this IServiceCollection services, string apiName, string url)
        {
            services.AddHttpClient(apiName, client =>
            {
                client.BaseAddress = new Uri(url);
                client.Timeout = new TimeSpan(0, 0, 30);
                client.DefaultRequestHeaders.Clear();
            });

            return services;
        }
    }
}
