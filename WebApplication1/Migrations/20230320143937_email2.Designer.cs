﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using WebApplication1;

#nullable disable

namespace WebApplication1.Migrations
{
    [DbContext(typeof(EmailDBContext))]
    [Migration("20230320143937_email2")]
    partial class email2
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.4")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("WebApplication1.Emails", b =>
                {
                    b.Property<int>("email_id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("email_id"));

                    b.Property<string>("Address")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<DateTime>("date")
                        .HasColumnType("timestamp without time zone");

                    b.Property<string>("status")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("email_id");

                    b.ToTable("Emails", (string)null);
                });
#pragma warning restore 612, 618
        }
    }
}
