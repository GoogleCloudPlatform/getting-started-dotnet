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

using System.Data.Entity.Migrations;

namespace GoogleCloudSamples.Migrations
{
    internal sealed class Configuration : DbMigrationsConfiguration<GoogleCloudSamples.Models.ApplicationDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
            if (LibUnityConfig.ChooseBookStoreFromConfig() == BookStoreFlag.MySql)
            {
                SetSqlGenerator("MySql.Data.MySqlClient",
                    new MySql.Data.Entity.MySqlMigrationSqlGenerator());
            }
            else if (LibUnityConfig.ChooseBookStoreFromConfig() == BookStoreFlag.SqlServer)
            {
                SetSqlGenerator("System.Data.SqlClient",
                    new System.Data.Entity.SqlServer.SqlServerMigrationSqlGenerator());
            }
        }
    }
}
