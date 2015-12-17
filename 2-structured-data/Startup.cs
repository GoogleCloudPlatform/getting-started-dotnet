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
using Microsoft.AspNet.Builder;
using Microsoft.AspNet.Hosting;
using Microsoft.Data.Entity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using GoogleCloudSamples.Models;

namespace GoogleCloudSamples
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
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
        }

        public IConfigurationRoot Configuration { get; set; }

        // This method gets called by the runtime. 
        // Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Add framework services.
            var entityFramework = services.AddEntityFramework();
            // Choose a database based on configuration.
            string sqlserverConnectionString;
            string npgsqlConnectionString;
            if (!string.IsNullOrWhiteSpace(npgsqlConnectionString =
                Configuration["Data:Npgsql:ConnectionString"]))
            {
                entityFramework.AddNpgsql()
                    .AddDbContext<ApplicationDbContext>(options =>
                        options.UseNpgsql(npgsqlConnectionString));
                services.Add(new ServiceDescriptor(typeof(IBookStore),
                    typeof(DbBookStore), ServiceLifetime.Scoped));
            }
            else if (!string.IsNullOrWhiteSpace(sqlserverConnectionString =
                Configuration["Data:SqlServer:ConnectionString"]))
            {
                entityFramework.AddSqlServer()
                    .AddDbContext<ApplicationDbContext>(options =>
                        options.UseSqlServer(sqlserverConnectionString));
                services.Add(new ServiceDescriptor(typeof(IBookStore),
                    typeof(DbBookStore), ServiceLifetime.Scoped));
            }
            else
            {
                throw new System.Exception(
                    "Please set a database connection string.");
            }
            services.AddMvc();
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
