using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Hosting;
namespace DockerApi
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateWebHostBuilder(args).Build().Run();
        }
        public static IWebHostBuilder CreateWebHostBuilder(string[] args)
        {
            var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
            var isProduction = environment == Microsoft.AspNetCore.Hosting.EnvironmentName.Production;
            if (isProduction)
            {
                string port = Environment.GetEnvironmentVariable("PORT") ?? "8080";
                string url = String.Concat("http://0.0.0.0:", port);
                return WebHost.CreateDefaultBuilder(args)
                    .UseUrls(url)
                    .UseStartup<Startup>();

            }
            return WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>();



        }

    }
}
