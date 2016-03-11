// Copyright(c) 2016 Google Inc.
//
// Licensed under the Apache License, Version 2.0 (the "License"); you may not
// use this file except in compliance with the License. You may obtain a copy of
// the License at
//
// http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS, WITHOUT
// WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. See the
// License for the specific language governing permissions and limitations under
// the License.

using GoogleCloudSamples.Models;
using GoogleCloudSamples.Services;
using Microsoft.Practices.Unity;
using System;
using System.Configuration;
using System.Data.Entity;
using System.Runtime.Serialization;

namespace GoogleCloudSamples
{
    public class ConfigurationException : Exception, ISerializable
    {
        public ConfigurationException(string message) : base(message)
        {
        }
    }

    /// <summary>
    /// We can store the book data in different places.  This flag tells us where we to store
    /// the book data.
    /// </summary>
    public enum BookStoreFlag
    {
        MySql,
        Datastore
    }

    /// <summary>
    /// Specifies the Unity configuration for the main container.
    /// </summary>
    public class UnityConfig
    {
        #region Unity Container

        private static readonly Lazy<IUnityContainer> s_container = new Lazy<IUnityContainer>(() =>
        {
            var container = new UnityContainer();
            RegisterTypes(container);
            return container;
        });

        /// <summary>
        /// Gets the configured Unity container.
        /// </summary>
        public static IUnityContainer GetConfiguredContainer()
        {
            return s_container.Value;
        }

        #endregion Unity Container

        public static BookStoreFlag ChooseBookStoreFromConfig()
        {
            string bookStore = Config.GetConfigVariable("GoogleCloudSamples:BookStore")?.ToLower();
            switch (bookStore)
            {
                case "datastore":
                    return BookStoreFlag.Datastore;

                case "mysql":
                    DbConfiguration.SetConfiguration(new MySql.Data.Entity.MySqlEFConfiguration());
                    return BookStoreFlag.MySql;

                default:
                    throw new ConfigurationException(
                         "Set the configuration variable GoogleCloudSamples:BookStore " +
                         "to datastore or mysql.");
            }
        }

        /// <summary>Registers the type mappings with the Unity container.</summary>
        /// <param name="container">The unity container to configure.</param>
        public static void RegisterTypes(IUnityContainer container)
        {
            ApplicationDbContextFactory factory;
            switch (ChooseBookStoreFromConfig())
            {
                case BookStoreFlag.Datastore:
                    container.RegisterInstance<IBookStore>(
                        new DatastoreBookStore(Config.GetConfigVariable("GoogleCloudSamples:ProjectId")));
                    break;

                case BookStoreFlag.MySql:
                    factory = new ApplicationDbContextFactory();
                    container.RegisterType<ApplicationDbContext>(
                        new InjectionFactory((x) => factory.Create()));
                    container.RegisterType<IBookStore, DbBookStore>();
                    break;
            }

            container.RegisterInstance<ImageUploader>(
                new ImageUploader(
                  Config.GetConfigVariable("GoogleCloudSamples:BucketName"),
                  Config.GetConfigVariable("GoogleCloudSamples:ApplicationName")
                )
            );
        }
    }
}