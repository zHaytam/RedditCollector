﻿// <auto-generated />
using System;
using Collector.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Collector.Migrations
{
    [DbContext(typeof(DatabaseContext))]
    [Migration("20181130165031_AddHeadImageToSubreddits")]
    partial class AddHeadImageToSubreddits
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.1.4-rtm-31024")
                .HasAnnotation("Relational:MaxIdentifierLength", 64);

            modelBuilder.Entity("Collector.Models.Subreddit", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime?>("Created");

                    b.Property<string>("HeaderImageUrl");

                    b.Property<string>("Name");

                    b.Property<string>("PublicDescription");

                    b.Property<int?>("SubscribersCount");

                    b.Property<string>("Title");

                    b.HasKey("Id");

                    b.ToTable("Subreddits");
                });
#pragma warning restore 612, 618
        }
    }
}