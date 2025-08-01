﻿// <auto-generated />
using System;
using System.Collections.Generic;
using Atoms.Core.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace Atoms.Infrastructure.Data.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20250719213303_AddGame")]
    partial class AddGame
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "9.0.2");

            modelBuilder.Entity("Atoms.Core.DTOs.GameDTO", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT");

                    b.Property<int>("AtomShape")
                        .HasColumnType("INTEGER");

                    b.Property<int>("ColourScheme")
                        .HasColumnType("INTEGER");

                    b.Property<DateTime>("CreatedDateUtc")
                        .HasColumnType("TEXT");

                    b.Property<bool>("IsActive")
                        .HasColumnType("INTEGER");

                    b.Property<DateTime>("LastUpdatedDateUtc")
                        .HasColumnType("TEXT");

                    b.Property<Guid>("LocalStorageId")
                        .HasColumnType("TEXT");

                    b.Property<int>("Move")
                        .HasColumnType("INTEGER");

                    b.Property<int>("Round")
                        .HasColumnType("INTEGER");

                    b.Property<string>("UserId")
                        .HasColumnType("TEXT");

                    b.ComplexProperty<Dictionary<string, object>>("Board", "Atoms.Core.DTOs.GameDTO.Board#BoardDTO", b1 =>
                        {
                            b1.IsRequired();

                            b1.Property<string>("Data")
                                .IsRequired()
                                .HasColumnType("TEXT");
                        });

                    b.ComplexProperty<Dictionary<string, object>>("Rng", "Atoms.Core.DTOs.GameDTO.Rng#RngDTO", b1 =>
                        {
                            b1.IsRequired();

                            b1.Property<int>("Iterations")
                                .HasColumnType("INTEGER");

                            b1.Property<int>("Seed")
                                .HasColumnType("INTEGER");
                        });

                    b.HasKey("Id");

                    b.HasIndex("LocalStorageId");

                    b.ToTable("Games");
                });

            modelBuilder.Entity("Atoms.Core.DTOs.PlayerDTO", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT");

                    b.Property<string>("AbbreviatedName")
                        .HasColumnType("TEXT");

                    b.Property<Guid>("GameId")
                        .HasColumnType("TEXT");

                    b.Property<string>("InviteCode")
                        .HasColumnType("TEXT");

                    b.Property<bool>("IsWinner")
                        .HasColumnType("INTEGER");

                    b.Property<Guid?>("LocalStorageId")
                        .HasColumnType("TEXT");

                    b.Property<string>("Name")
                        .HasColumnType("TEXT");

                    b.Property<int>("Number")
                        .HasColumnType("INTEGER");

                    b.Property<int>("Type")
                        .HasColumnType("INTEGER");

                    b.Property<string>("UserId")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("GameId");

                    b.ToTable("Players");
                });

            modelBuilder.Entity("Atoms.Core.DTOs.PlayerDTO", b =>
                {
                    b.HasOne("Atoms.Core.DTOs.GameDTO", "Game")
                        .WithMany("Players")
                        .HasForeignKey("GameId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Game");
                });

            modelBuilder.Entity("Atoms.Core.DTOs.GameDTO", b =>
                {
                    b.Navigation("Players");
                });
#pragma warning restore 612, 618
        }
    }
}
