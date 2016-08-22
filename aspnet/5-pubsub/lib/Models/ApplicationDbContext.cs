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

using System.Data.Common;
using System.Data.Entity;

namespace GoogleCloudSamples.Models
{
    // [START dbset]
    public class ApplicationDbContext : DbContext
    {
        // [START_EXCLUDE]
        private static readonly string s_mySqlServerBaseName = "LocalMySqlServer";
        private static readonly string s_sqlServerBaseName = "LocalSqlServer";
        // [END_EXCLUDE]
        public DbSet<Book> Books { get; set; }
        // [END dbset]

        /// <summary>
        /// Needed to instantiate ApplicationDbContext with a connection string
        /// pulled from an environment variable.
        /// </summary>
        /// <param name="connection"></param>
        internal ApplicationDbContext(DbConnection connection) : base(connection, true)
        {
        }

        /// <summary>
        /// Pulls connection string from Web.config.
        /// </summary>
        internal ApplicationDbContext() : base("name=" +
            ((LibUnityConfig.ChooseBookStoreFromConfig() == BookStoreFlag.MySql)
            ? s_mySqlServerBaseName : s_sqlServerBaseName))
        {
        }
    }
}
