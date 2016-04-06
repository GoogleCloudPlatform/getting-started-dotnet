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

namespace GoogleCloudSamples.Services
{
    /// <summary>
    /// Using a sophisticated logger like log4net is beyond the scope of this sample.
    /// So, we use a simple logging interface.
    /// </summary>
    public interface ISimpleLogger
    {
        void LogVerbose(string message);

        void LogError(string message, Exception e);
    }
}