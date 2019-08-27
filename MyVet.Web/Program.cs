using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using MyVet.Web.Data;

namespace MyVet.Web
{
    public class Program
    {
        public static void Main(string[] args)
        {
            //Para ejecutar e SeedDb para llenar los catálogos iniciales
            var host = CreateWebHostBuilder(args).Build();
            RunSeeding(host);
            host.Run();
        }

        private static void RunSeeding(IWebHost host)
        {
            var scopeFactory = host.Services.GetService<IServiceScopeFactory>();
            using (var scope = scopeFactory.CreateScope())
            {
                var seeder = scope.ServiceProvider.GetService<SeedDb>();
                seeder.SeedAsync().Wait();
            }
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args)
        {
            return WebHost.CreateDefaultBuilder(args).UseStartup<Startup>();
        }
    }
}