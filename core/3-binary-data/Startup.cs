// Copyright 2015 Google Inc.
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
//
using GoogleCloudSamples.Models;
using Microsoft.AspNet.Builder;
using Microsoft.AspNet.Hosting;
using Microsoft.Data.Entity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Diagnostics;



namespace GoogleCloudSamples
{
    public class Startup
    {
        private ILoggerFactory _loggerFactory;
        public Startup(IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            _loggerFactory = loggerFactory;

            // Set up configuration sources.
            var builder = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json",
                optional: true);

            if (env.IsDevelopment())
            {
                // For more details on using the user secret store see
                // http://go.microsoft.com/fwlink/?LinkID=532709
                builder.AddUserSecrets();
            }

            builder.AddEnvironmentVariables();
            Configuration = builder.Build();
            _loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            _loggerFactory.AddDebug();
        }

        public IConfigurationRoot Configuration { get; set; }

        // This method gets called by the runtime.
        // Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            var logger = _loggerFactory.CreateLogger("ConfigureServices");
            services.AddAntiforgery();
            // Choose a a backend to store the books.
            switch (Configuration["Data:BookStore"]?.ToLower())
            {
                case "sqlserver":
                    AddSqlServer(services);
                    logger.LogInformation("Storing book data in SQL Server.");
                    break;

                case "postgres":
                    AddPostgres(services);
                    logger.LogInformation("Storing book data in PostgreSql.");
                    break;

                case "datastore":
                    AddDatastore(services);
                    logger.LogInformation("Storing book data in Datastore.");
                    break;

                default:
                    Halt("No bookstore backend selected.\n" +
                        "Set the configuration variable Data:BookStore to " +
                        "one of the following: sqlserver, postgres, datastore.");
                    break;
            }
            services.AddMvc();
            services.AddImageUploader(Configuration.GetOrThrow("GoogleCloudSamples:BucketName"),
                Configuration["GoogleCloudSamples:ApplicationName"] ?? "GettingStarted.Net");
        }

        public void Halt(string message)
        {
            var logger = _loggerFactory.CreateLogger("Halt");
            logger.LogCritical(message);
            if (Debugger.IsAttached)
                Debugger.Break();
            Environment.Exit(-1);
        }

        private void AddDatastore(IServiceCollection services)
        {
            string projectId = Configuration["GOOGLE_PROJECT_ID"];
            if (string.IsNullOrWhiteSpace(projectId))
                Halt("Set the configuration variable GOOGLE_PROJECT_ID.");
            services.Add(new ServiceDescriptor(typeof(IBookStore),
                (x) => new DatastoreBookStore(projectId),
                ServiceLifetime.Singleton));
        }

        private void AddSqlServer(IServiceCollection services)
        {
            var entityFramework = services.AddEntityFramework();
            string sqlserverConnectionString =
                Configuration["Data:SqlServer:ConnectionString"];
            if (string.IsNullOrWhiteSpace(sqlserverConnectionString))
                Halt("Set the configuration variable Data:SqlServer:ConnectionString.");
            entityFramework.AddSqlServer()
                .AddDbContext<ApplicationDbContext>(options =>
                    options.UseSqlServer(sqlserverConnectionString));
            services.AddScoped(typeof(IBookStore), typeof(DbBookStore));
        }

        private void AddPostgres(IServiceCollection services)
        {
            var entityFramework = services.AddEntityFramework();
            string npgsqlConnectionString =
                Configuration["Data:Npgsql:ConnectionString"];
            if (string.IsNullOrWhiteSpace(npgsqlConnectionString))
                Halt("Set the configuration variable Data:Npgsql:ConnectionString.");
            entityFramework.AddNpgsql()
                .AddDbContext<ApplicationDbContext>(options =>
                    options.UseNpgsql(npgsqlConnectionString));
            services.AddScoped(typeof(IBookStore), typeof(DbBookStore));
        }

        // This method gets called by the runtime. Use this method to configure
        // the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env,
            ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            if (env.IsDevelopment())
            {
                app.UseBrowserLink();
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");

                // For more details on creating database during deployment see
                // http://go.microsoft.com/fwlink/?LinkID=615859
                try
                {
                    using (var serviceScope = app.ApplicationServices
                        .GetRequiredService<IServiceScopeFactory>()
                        .CreateScope())
                    {
                        serviceScope.ServiceProvider
                            .GetService<ApplicationDbContext>()
                            .Database.Migrate();
                    }
                }
                catch { }
            }

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }

        // Entry point for the application.
        public static void Main(string[] args) =>
            WebApplication.Run<Startup>(args);
    }
}