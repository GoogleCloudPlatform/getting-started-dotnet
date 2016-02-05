using System;
using Microsoft.Data.Entity;
using Microsoft.Data.Entity.Infrastructure;
using Microsoft.Data.Entity.Metadata;
using Microsoft.Data.Entity.Migrations;
using GoogleCloudSamples.Models;

namespace _2structureddata.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20160120182607_init")]
    partial class init
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.0-rc1-16348");

            modelBuilder.Entity("GoogleCloudSamples.Models.Book", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Author");

                    b.Property<string>("CreatedBy");

                    b.Property<string>("CreatedById");

                    b.Property<string>("Description");

                    b.Property<string>("ImageUrl");

                    b.Property<DateTime?>("PublishedDate");

                    b.Property<string>("Title");

                    b.HasKey("Id");
                });
        }
    }
}
