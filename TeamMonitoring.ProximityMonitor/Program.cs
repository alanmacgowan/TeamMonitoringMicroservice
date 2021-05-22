using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using System.Reflection;
using TeamMonitoring.Common.Logging;

namespace TeamMonitoring.ProximityMonitor
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .AddLogging(Assembly.GetExecutingAssembly().GetName().Name)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
