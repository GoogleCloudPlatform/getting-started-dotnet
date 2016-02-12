// Copyright 2016 Google Inc. All Rights Reserved.
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//      http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
using GoogleCloudSamples.Models;
using Microsoft.Data.Entity;
using Microsoft.Data.Entity.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;

namespace GoogleCloudSamples
{
    public class ConfigurationException : Exception
    {
        public ConfigurationException(string message) : base(message) { }
    }

    /// <summary>
    /// Configures the IBookStore service.
    /// </summary>
    public static class ConfigurationMethodExtensions
    {
        /// <summary>
        /// Throws an exception of the key is not in the configuration.
        /// </summary>
        /// <param name="configuration"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string GetOrThrow(this IConfigurationRoot configuration, string key)
        {
            var value = configuration[key];
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new ConfigurationException($"You must set the configuration variable {key}.");
            }
            return value;
        }

        /// <summary>
        /// Adds an IBookStore implementation to services depending on configuration.
        /// </summary>
        public static void AddBookStore(this IServiceCollection services,
            IConfigurationRoot configuration, ILoggerFactory loggerFactory)
        {
            var logger = loggerFactory.CreateLogger("AddBookStore");
            EntityFrameworkServicesBuilder entityFramework;
            // Choose a a backend to store the books.
            switch (configuration["Data:BookStore"]?.ToLower())
            {
                case "sqlserver":
                    logger.LogInformation("Storing book data in SQL Server.");
                    entityFramework = services.AddEntityFramework();
                    string sqlserverConnectionString =
                        configuration.GetOrThrow("Data:SqlServer:ConnectionString");
                    entityFramework.AddSqlServer()
                        .AddDbContext<ApplicationDbContext>(options =>
                            options.UseSqlServer(sqlserverConnectionString));
                    services.AddScoped(typeof(IBookStore), typeof(DbBookStore));
                    break;

                case "postgres":
                    logger.LogInformation("Storing book data in PostgreSql.");
                    entityFramework = services.AddEntityFramework();
                    string npgsqlConnectionString =
                        configuration.GetOrThrow("Data:Npgsql:ConnectionString");
                    entityFramework.AddNpgsql()
                        .AddDbContext<ApplicationDbContext>(options =>
                            options.UseNpgsql(npgsqlConnectionString));
                    services.AddScoped(typeof(IBookStore), typeof(DbBookStore));
                    break;

                case "datastore":
                    logger.LogInformation("Storing book data in Datastore.");
                    services.AddSingleton<IBookStore>((IServiceProvider provider) =>
                    {
                        return new DatastoreBookStore(
                            configuration.GetOrThrow("GOOGLE_PROJECT_ID"));
                    });
                    break;

                default:
                    throw new ConfigurationException("No bookstore backend selected.\n" +
                        "Set the configuration variable Data:BookStore to " +
                        "one of the following: sqlserver, postgres, datastore.");
            }
        }
        public static void AddBookDetailLookup(this IServiceCollection services, string projectId,
            Action<BookDetailLookup.Options> optionSetter = null)
        {
            services.AddSingleton<BookDetailLookup>(provider =>
            {
                var options = new BookDetailLookup.Options();
                if (optionSetter != null) optionSetter(options);
                var bookDetailLookup = new BookDetailLookup(
                    projectId, provider.GetService<ILoggerFactory>(), options);
                bookDetailLookup.CreateTopicAndSubscription();
                return bookDetailLookup;
            });
        }
    }
}