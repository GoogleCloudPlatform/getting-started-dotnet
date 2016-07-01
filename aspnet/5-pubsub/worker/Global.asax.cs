﻿// Copyright(c) 2016 Google Inc.
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
using System.Threading;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace GoogleCloudSamples
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            // Launch a thread that watches the book detail subscription.
            var container = App_Start.UnityConfig.GetConfiguredContainer();
            LibUnityConfig.RegisterTypes(container);
            var bookDetailLookup = new BookDetailLookup(LibUnityConfig.ProjectId,
                logger: LogTicker.Instance);
            bookDetailLookup.CreateTopicAndSubscription();
            var pullTask = bookDetailLookup.StartPullLoop(container.Resolve<IBookStore>(),
                new CancellationTokenSource().Token);
        }
    }
}