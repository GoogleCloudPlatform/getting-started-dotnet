﻿// Copyright (c) 2019 Google LLC.
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

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;

namespace HelloWorld
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            // This is not a production app so we always use the developer exception page.
            // You should ensure that your app uses the correct error page depending on the environment
            // it runs in.
            app.UseDeveloperExceptionPage();

            app.Run(async (context) =>
            {
                await context.Response.WriteAsync("Hello World.");
            });
        }
    }
}
