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

using System;
using System.Configuration;

namespace GoogleCloudSamples
{
    public static class Config
    {
        /// <summary>
        /// Looks for variable in app settings.
        /// Throws an exception of the key is not in the configuration.
        /// </summary>
        public static string GetConfigVariable(string key)
        {
            string value = ConfigurationManager.AppSettings[key];
            if (value == null)
                throw new ConfigurationException($"You must set the configuration variable {key}.");
            return value;
        }
    }
}