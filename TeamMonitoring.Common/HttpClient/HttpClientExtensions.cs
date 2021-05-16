using Microsoft.Extensions.DependencyInjection;
using System;
using System.Net.Http.Headers;

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
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            });

            return services;
        }
    }
}
