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

using GoogleCloudSamples.Hubs;
using GoogleCloudSamples.Services;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using System;

namespace GoogleCloudSamples
{
    /// <summary>
    /// A singleton that sends log messages to listening clients.
    /// </summary>
    public class LogTicker : ISimpleLogger
    {
        // Singleton instance
        private readonly static Lazy<LogTicker> s_instance = new Lazy<LogTicker>(() => new LogTicker(GlobalHost.ConnectionManager.GetHubContext<LogHub>().Clients));

        /// <summary>
        /// The message most recent message passed to LogVerbose().
        /// </summary>
        private string _lastMessage;
        private readonly object _lastMessageLock = new Object();

        private LogTicker(IHubConnectionContext<dynamic> clients)
        {
            Clients = clients;
        }

        public static LogTicker Instance
        {
            get
            {
                return s_instance.Value;
            }
        }

        private IHubConnectionContext<dynamic> Clients
        {
            get;
            set;
        }

        public void LogVerbose(string message)
        {
            lock (_lastMessageLock) _lastMessage = message;
            Clients.All.logVerbose(message);
        }

        public void LogError(string message, Exception e)
        {
            Clients.All.logError(message, e.Message);
        }

        /// <summary>
        /// Get the most recent message written to the log.
        /// </summary>
        /// <returns>The most recent message written to the log.</returns>
        public string GetLastMessage()
        {
            lock (_lastMessageLock) return _lastMessage;
        }
    }
}