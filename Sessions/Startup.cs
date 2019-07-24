using Google.Cloud.AspNetCore.DataProtection.Kms;
using Google.Cloud.AspNetCore.DataProtection.Storage;
using Google.Cloud.AspNetCore.Firestore.DistributedCache;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.DataProtection;

namespace Sessions
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            // Antiforgery tokens require data protection.
            services.AddDataProtection()
                // Store keys in Cloud Storage so that multiple instances
                // of the web application see the same keys.
                .PersistKeysToGoogleCloudStorage(
                    Configuration["DataProtection:Bucket"],
                    Configuration["DataProtection:Object"])
                // Protect the keys with Google KMS for encryption and fine-
                // grained access control.
                .ProtectKeysWithGoogleKms(
                    Configuration["DataProtection:KmsKeyName"]);
            services.AddFirestoreDistributedCache()
                .AddFirestoreDistributedCacheGarbageCollector();
            services.AddSession();
        }

        Random _random = new Random();

        private static readonly string[] _greetings = 
            {"Hello World", "Hallo Welt", "Hola mundo", 
            "Salut le Monde", "Ciao Mondo"};

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.UseSession();

            app.Run(async (context) =>
            {
                // Retreive the # of views from the session.
                var views = context.Session.GetInt32("views").GetValueOrDefault();
                views += 1;
                context.Session.SetInt32("views" , views);
                // Retrieve the randomly selected greeting from the session.
                var greeting = context.Session.GetString("greeting");
                if (greeting is null)
                {
                    greeting = _greetings[_random.Next(_greetings.Length)];
                    context.Session.SetString("greeting", greeting);
                }
                await context.Response.WriteAsync($"{views} views for {greeting}.");
            });
        }
    }
}
