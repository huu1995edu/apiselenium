using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
namespace DockerApi {
    public class Program {
        public static void Main (string[] args) {
            CreateWebHostBuilder (args).Build ().Run ();
        }
        public static IWebHostBuilder CreateWebHostBuilder (string[] args) {
            string url = Environment.GetEnvironmentVariable ("ASPNETCORE_URLS");
            return WebHost.CreateDefaultBuilder (args)
                .UseUrls (url)
                .UseStartup<Startup> ();
        }

    }
} 