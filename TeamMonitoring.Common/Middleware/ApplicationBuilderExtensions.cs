using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using TeamMonitoring.Common.Infrastructure;

namespace TeamMonitoring.Common.Middleware
{
    public static class ApplicationBuilderExtensions
    {

        public static IApplicationBuilder UseExceptionHandling(this IApplicationBuilder appBuilder, IWebHostEnvironment env, ILogger logger)
        {
            if (env.IsDevelopment())
            {
                //appBuilder.UseDeveloperExceptionPage();
            }

            appBuilder.UseMiddleware<ExceptionMiddleware>(logger);

            return appBuilder;
        }
    }
}
