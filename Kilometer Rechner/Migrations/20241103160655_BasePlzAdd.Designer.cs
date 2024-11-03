﻿// <auto-generated />
using System;
using Kilometer_Rechner;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace Kilometer_Rechner.Migrations
{
    [DbContext(typeof(DbContext))]
    [Migration("20241103160655_BasePlzAdd")]
    partial class BasePlzAdd
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.10")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("Kilometer_Rechner.Models.Calculation", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<double>("AirLineKm")
                        .HasColumnType("float");

                    b.Property<int>("BasePlz")
                        .HasColumnType("int");

                    b.Property<DateTime>("CalcDate")
                        .HasColumnType("datetime2");

                    b.Property<int>("IdPlz")
                        .HasColumnType("int");

                    b.Property<double>("RouteLineKm")
                        .HasColumnType("float");

                    b.HasKey("Id");

                    b.ToTable("Calculations", "dbo");
                });

            modelBuilder.Entity("Kilometer_Rechner.Models.CityModel", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<double>("Latitude")
                        .HasColumnType("float");

                    b.Property<double>("Longitude")
                        .HasColumnType("float");

                    b.Property<string>("Ort")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<string>("PLZ")
                        .IsRequired()
                        .HasMaxLength(10)
                        .HasColumnType("nvarchar(10)");

                    b.HasKey("Id");

                    b.ToTable("Cities", "dbo");
                });
#pragma warning restore 612, 618
        }
    }
}