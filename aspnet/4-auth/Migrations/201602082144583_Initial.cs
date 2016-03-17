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
    public partial class Initial : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Books",
                c => new
                {
                    Id = c.Long(nullable: false, identity: true),
                    Title = c.String(nullable: false, unicode: false),
                    Author = c.String(unicode: false),
                    PublishedDate = c.DateTime(precision: 0),
                    ImageUrl = c.String(unicode: false),
                    Description = c.String(unicode: false),
                    CreatedById = c.String(unicode: false),
                })
                .PrimaryKey(t => t.Id);
        }

        public override void Down()
        {
            DropTable("dbo.Books");
        }
    }
}
