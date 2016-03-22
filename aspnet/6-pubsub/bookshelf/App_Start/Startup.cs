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

using System;
using System.Threading.Tasks;
using System.Security.Claims;

using Owin;
using Microsoft.Owin;
using Microsoft.Owin.Security.Google;
using Microsoft.Owin.Security.Cookies;
using Microsoft.AspNet.Identity;

[assembly: OwinStartupAttribute(typeof(GoogleCloudSamples.Startup))]

namespace GoogleCloudSamples
{
    public class Startup
    {
        /// <summary>Configure Google OAuth2 authentication</summary>
        /// <remarks>
        /// OAauth Client Id and Client Secret must be set in application configuration
        /// </remarks>
        public void Configuration(IAppBuilder app)
        {
            // [START cookie_authentication]
            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                AuthenticationType = DefaultAuthenticationTypes.ExternalCookie
            });
            app.UseExternalSignInCookie(DefaultAuthenticationTypes.ExternalCookie);
            app.UseTwoFactorSignInCookie(DefaultAuthenticationTypes.TwoFactorCookie, TimeSpan.FromMinutes(5));
            app.UseTwoFactorRememberBrowserCookie(DefaultAuthenticationTypes.TwoFactorRememberBrowserCookie);
            // [END cookie_authentication]

            // [START configure_google_auth_client]
            var authenticationOptions = new GoogleOAuth2AuthenticationOptions()
            {
                ClientId = Config.GetConfigVariable("GoogleCloudSamples:AuthClientId"),
                ClientSecret = Config.GetConfigVariable("GoogleCloudSamples:AuthClientSecret"),
            };
            // [END configure_google_auth_client]

            // [START configure_google_auth_scopes]
            // Add scope to access user's basic profile information
            authenticationOptions.Scope.Add("profile");
            // [END configure_google_auth_scopes]

            authenticationOptions.Provider = new GoogleOAuth2AuthenticationProvider()
            {
                // [START read_google_profile_image_url]
                // After OAuth authentication completes successfully,
                // read user's profile image URL from the profile
                // response data and add it to the current user identity
                OnAuthenticated = context =>
                {
                    var profileUrl = context.User["image"]["url"].ToString();
                    context.Identity.AddClaim(new Claim(ClaimTypes.Uri, profileUrl));
                    return Task.FromResult(0);
                }
                // [END read_google_profile_image_url]
            };

            app.UseGoogleAuthentication(authenticationOptions);
        }
    }
}
