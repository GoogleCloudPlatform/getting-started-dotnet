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
using GoogleCloudSamples.Services;
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
    /// Configures the ImageUploader service.
    /// </summary>
    public static class ImageUploaderServiceExtensions
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
        /// Adds an ImageUploader implementation to services depending on configuration.
        /// </summary>
        public static void AddImageUploader(this IServiceCollection services, 
            string bucketName, string applicationName)
        {
            services.AddInstance<ImageUploader>(new ImageUploader(bucketName, 
                applicationName));
        }
    }
}