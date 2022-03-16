﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Shapr3D_Converter.Models;

namespace Shapr3D_Converter.Models.Migrations
{
    [DbContext(typeof(Shapr3dDbContext))]
    [Migration("20220310162334_FirstMigration")]
    partial class FirstMigration
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "3.1.22");

            modelBuilder.Entity("Shapr3D_Converter.Models.ModelEntity", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT");

                    b.Property<byte[]>("FileBytes")
                        .HasColumnType("BLOB");

                    b.Property<ulong>("FileSize")
                        .HasColumnType("INTEGER");

                    b.Property<TimeSpan?>("ObjConversionTime")
                        .HasColumnType("TEXT");

                    b.Property<bool>("ObjConverted")
                        .HasColumnType("INTEGER");

                    b.Property<byte[]>("ObjFileBytes")
                        .HasColumnType("BLOB");

                    b.Property<string>("OriginalPath")
                        .HasColumnType("TEXT");

                    b.Property<TimeSpan?>("StepConversionTime")
                        .HasColumnType("TEXT");

                    b.Property<bool>("StepConverted")
                        .HasColumnType("INTEGER");

                    b.Property<byte[]>("StepFileBytes")
                        .HasColumnType("BLOB");

                    b.Property<TimeSpan?>("StlConversionTime")
                        .HasColumnType("TEXT");

                    b.Property<bool>("StlConverted")
                        .HasColumnType("INTEGER");

                    b.Property<byte[]>("StlFileBytes")
                        .HasColumnType("BLOB");

                    b.Property<byte[]>("ThumbnailBytes")
                        .HasColumnType("BLOB");

                    b.HasKey("Id");

                    b.ToTable("ModelEntities");
                });
#pragma warning restore 612, 618
        }
    }
}