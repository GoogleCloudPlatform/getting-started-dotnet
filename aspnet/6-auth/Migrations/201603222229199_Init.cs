namespace GoogleCloudSamples.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Init : DbMigration
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
