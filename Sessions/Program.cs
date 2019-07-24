using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Sessions
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateWebHostBuilder(args).Build().Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>()
                .UsePortEnvironmentVariable();
    }

    static class ProgramExtensions
    {
        // Google Cloud Run sets the PORT environment variable to tell this
        // process which port to listen to.
        public static IWebHostBuilder UsePortEnvironmentVariable(
            this IWebHostBuilder builder)
        {
            string port = Environment.GetEnvironmentVariable("PORT");
            if (!string.IsNullOrEmpty(port))
            {
                builder.UseUrls($"http://0.0.0.0:{port}");
            }
            return builder;
        }
    }
}
