﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using Pgvector;
using backend;

#nullable disable

namespace backend.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20240216232652_ObjectId")]
    partial class ObjectId
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.0")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.HasPostgresExtension(modelBuilder, "vector");
            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("backend.Models.Entity.Image", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("text");

                    b.Property<Vector>("CaptionEmbedding")
                        .HasColumnType("vector(384)");

                    b.Property<string>("FileExtension")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("GeneratedCaption")
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("CaptionEmbedding")
                        .HasAnnotation("Npgsql:StorageParameter:lists", 100);

                    NpgsqlIndexBuilderExtensions.HasMethod(b.HasIndex("CaptionEmbedding"), "ivfflat");
                    NpgsqlIndexBuilderExtensions.HasOperators(b.HasIndex("CaptionEmbedding"), new[] { "vector_l2_ops" });

                    b.ToTable("Images");
                });

            modelBuilder.Entity("backend.Models.Entity.Reel", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("text");

                    b.Property<string>("AudioTranscription")
                        .HasColumnType("text");

                    b.Property<int?>("Duration")
                        .HasColumnType("integer");

                    b.Property<string>("FileExtension")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("Reels");
                });
#pragma warning restore 612, 618
        }
    }
}
