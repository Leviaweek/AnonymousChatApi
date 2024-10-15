﻿// <auto-generated />
using System;
using AnonymousChatApi.Databases;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace AnonymousChatApi.Migrations
{
    [DbContext(typeof(AnonymousChatDbContext))]
    [Migration("20241015175149_InitialCreate")]
    partial class InitialCreate
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.10")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("AnonymousChatApi.Databases.Models.Chat", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<long>("Id"));

                    b.Property<DateTimeOffset>("CreatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("character varying(50)");

                    b.HasKey("Id");

                    b.HasIndex("Id")
                        .IsUnique();

                    b.ToTable("Chats", "public");
                });

            modelBuilder.Entity("AnonymousChatApi.Databases.Models.ChatUser", b =>
                {
                    b.Property<long>("UserId")
                        .HasColumnType("bigint");

                    b.Property<long>("ChatId")
                        .HasColumnType("bigint");

                    b.HasKey("UserId", "ChatId");

                    b.HasIndex("ChatId");

                    b.ToTable("ChatUsers", "public");
                });

            modelBuilder.Entity("AnonymousChatApi.Databases.Models.MessageBase", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<long>("Id"));

                    b.Property<long>("ChatId")
                        .HasColumnType("bigint");

                    b.Property<DateTimeOffset>("TimeStamp")
                        .HasColumnType("timestamp with time zone");

                    b.Property<long>("UserId")
                        .HasColumnType("bigint");

                    b.HasKey("Id");

                    b.HasIndex("ChatId");

                    b.HasIndex("UserId");

                    b.ToTable("MessageBases", "public");
                });

            modelBuilder.Entity("AnonymousChatApi.Databases.Models.NotifyMessage", b =>
                {
                    b.Property<long>("MessageId")
                        .HasColumnType("bigint");

                    b.Property<int>("Type")
                        .HasColumnType("integer");

                    b.HasKey("MessageId");

                    b.ToTable("NotifyMessages", "public");
                });

            modelBuilder.Entity("AnonymousChatApi.Databases.Models.TextMessage", b =>
                {
                    b.Property<long>("MessageId")
                        .HasColumnType("bigint");

                    b.Property<string>("Text")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("MessageId");

                    b.ToTable("TextMessages", "public");
                });

            modelBuilder.Entity("AnonymousChatApi.Databases.Models.User", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<long>("Id"));

                    b.Property<long>("LastReadMessageId")
                        .HasColumnType("bigint");

                    b.Property<string>("Login")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("character varying(50)");

                    b.Property<string>("Password")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("character varying(50)");

                    b.Property<DateTimeOffset>("TimeStamp")
                        .HasColumnType("timestamp with time zone");

                    b.HasKey("Id");

                    b.HasIndex("Id")
                        .IsUnique();

                    b.ToTable("Users", "public");
                });

            modelBuilder.Entity("AnonymousChatApi.Databases.Models.ChatUser", b =>
                {
                    b.HasOne("AnonymousChatApi.Databases.Models.Chat", "Chat")
                        .WithMany("ChatUsers")
                        .HasForeignKey("ChatId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("AnonymousChatApi.Databases.Models.User", null)
                        .WithMany("ChatUsers")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Chat");
                });

            modelBuilder.Entity("AnonymousChatApi.Databases.Models.MessageBase", b =>
                {
                    b.HasOne("AnonymousChatApi.Databases.Models.Chat", null)
                        .WithMany("Messages")
                        .HasForeignKey("ChatId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("AnonymousChatApi.Databases.Models.User", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("AnonymousChatApi.Databases.Models.NotifyMessage", b =>
                {
                    b.HasOne("AnonymousChatApi.Databases.Models.MessageBase", null)
                        .WithOne("NotifyMessage")
                        .HasForeignKey("AnonymousChatApi.Databases.Models.NotifyMessage", "MessageId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("AnonymousChatApi.Databases.Models.TextMessage", b =>
                {
                    b.HasOne("AnonymousChatApi.Databases.Models.MessageBase", null)
                        .WithOne("TextMessage")
                        .HasForeignKey("AnonymousChatApi.Databases.Models.TextMessage", "MessageId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("AnonymousChatApi.Databases.Models.Chat", b =>
                {
                    b.Navigation("ChatUsers");

                    b.Navigation("Messages");
                });

            modelBuilder.Entity("AnonymousChatApi.Databases.Models.MessageBase", b =>
                {
                    b.Navigation("NotifyMessage");

                    b.Navigation("TextMessage");
                });

            modelBuilder.Entity("AnonymousChatApi.Databases.Models.User", b =>
                {
                    b.Navigation("ChatUsers");
                });
#pragma warning restore 612, 618
        }
    }
}
