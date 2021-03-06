using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;
using System;

namespace TeamMonitoring.Common.Redis
{
    public static class RedisExtensions
    {
        public static IServiceCollection AddRedisConnectionMultiplexer(this IServiceCollection services, IConfiguration config)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            if (config == null)
            {
                throw new ArgumentNullException(nameof(config));
            }

            var redisConfig = config.GetSection("redis:configstring").Value;

            services.AddSingleton(typeof(IConnectionMultiplexer), ConnectionMultiplexer.ConnectAsync(redisConfig).Result);
            return services;
        }
    }
}