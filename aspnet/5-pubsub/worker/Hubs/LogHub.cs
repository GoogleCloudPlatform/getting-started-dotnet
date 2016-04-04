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

using Microsoft.AspNet.SignalR;

namespace GoogleCloudSamples.Hubs
{
    /// <summary>
    /// A SignalR Hub that wraps LogTicker.
    /// </summary>
    public class LogHub : Hub
    {
        /// <summary>
        /// Get the most recent message written to the log.
        /// </summary>
        /// <returns>The most recent message written to the log.</returns>
        public string GetLastMessage()
        {
            return LogTicker.Instance.GetLastMessage();
        }
    }
}