﻿// <auto-generated />
using CUNAMUTUAL_TAKEHOME;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace CUNAMUTUAL_TAKEHOME.Migrations
{
    [DbContext(typeof(MyContext))]
    [Migration("20220117012603_v2")]
    partial class v2
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "3.1.22");

            modelBuilder.Entity("CUNAMUTUAL_TAKEHOME.ServiceItem", b =>
                {
                    b.Property<string>("Identifier")
                        .HasColumnType("TEXT");

                    b.Property<int>("Status")
                        .HasColumnType("INTEGER");

                    b.HasKey("Identifier");

                    b.ToTable("ServiceItems");
                });
#pragma warning restore 612, 618
        }
    }
}