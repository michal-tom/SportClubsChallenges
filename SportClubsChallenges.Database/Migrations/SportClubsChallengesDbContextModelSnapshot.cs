﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using SportClubsChallenges.Database.Data;

namespace SportClubsChallenges.Database.Migrations
{
    [DbContext(typeof(SportClubsChallengesDbContext))]
    partial class SportClubsChallengesDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "3.1.11")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("SportClubsChallenges.Database.Entities.ActivityType", b =>
                {
                    b.Property<byte>("Id")
                        .HasColumnType("tinyint");

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("ActivityTypes");

                    b.HasData(
                        new
                        {
                            Id = (byte)1,
                            Name = "Run"
                        },
                        new
                        {
                            Id = (byte)2,
                            Name = "Ride"
                        },
                        new
                        {
                            Id = (byte)3,
                            Name = "Swim"
                        },
                        new
                        {
                            Id = (byte)4,
                            Name = "VirtualRide"
                        },
                        new
                        {
                            Id = (byte)5,
                            Name = "Walk"
                        },
                        new
                        {
                            Id = (byte)6,
                            Name = "Other"
                        });
                });

            modelBuilder.Entity("SportClubsChallenges.Database.Entities.Athlete", b =>
                {
                    b.Property<long>("Id")
                        .HasColumnType("bigint");

                    b.Property<long>("AthleteStravaTokenId")
                        .HasColumnType("bigint");

                    b.Property<string>("City")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Country")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("CreationDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("FirstName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Gender")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("IconUrlLarge")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("IconUrlMedium")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("LastLoginDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("LastName")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("AthleteStravaTokenId");

                    b.ToTable("Athletes");

                    b.HasData(
                        new
                        {
                            Id = 1L,
                            AthleteStravaTokenId = 1L,
                            CreationDate = new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                            FirstName = "John",
                            LastLoginDate = new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                            LastName = "Smith"
                        });
                });

            modelBuilder.Entity("SportClubsChallenges.Database.Entities.AthleteStravaToken", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("AccessToken")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTimeOffset>("ExpirationDate")
                        .HasColumnType("datetimeoffset");

                    b.Property<DateTimeOffset>("LastUpdateDate")
                        .HasColumnType("datetimeoffset");

                    b.Property<string>("RefreshToken")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("TokenType")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("AthleteStravaTokens");

                    b.HasData(
                        new
                        {
                            Id = 1L,
                            AccessToken = "00000000-0000-0000-0000-000000000000",
                            ExpirationDate = new DateTimeOffset(new DateTime(2021, 1, 31, 18, 41, 7, 53, DateTimeKind.Unspecified).AddTicks(2776), new TimeSpan(0, 1, 0, 0, 0)),
                            LastUpdateDate = new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)),
                            RefreshToken = "00000000-0000-0000-0000-000000000000"
                        });
                });

            modelBuilder.Entity("SportClubsChallenges.Database.Entities.Challenge", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<byte>("ChallengeType")
                        .HasColumnType("tinyint");

                    b.Property<long>("ClubId")
                        .HasColumnType("bigint");

                    b.Property<DateTime>("CreationDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("Description")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("EditionDate")
                        .HasColumnType("datetime2");

                    b.Property<DateTime>("EndDate")
                        .HasColumnType("datetime2");

                    b.Property<bool>("IncludeOnlyGpsActivities")
                        .HasColumnType("bit");

                    b.Property<bool>("IsActive")
                        .HasColumnType("bit");

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(max)");

                    b.Property<long>("OwnerId")
                        .HasColumnType("bigint");

                    b.Property<bool>("PreventManualActivities")
                        .HasColumnType("bit");

                    b.Property<DateTime>("StartDate")
                        .HasColumnType("datetime2");

                    b.HasKey("Id");

                    b.HasIndex("ClubId");

                    b.HasIndex("OwnerId");

                    b.ToTable("Challenges");
                });

            modelBuilder.Entity("SportClubsChallenges.Database.Entities.ChallengeActivityType", b =>
                {
                    b.Property<long>("ChallengeId")
                        .HasColumnType("bigint");

                    b.Property<byte>("ActivityTypeId")
                        .HasColumnType("tinyint");

                    b.HasKey("ChallengeId", "ActivityTypeId");

                    b.HasIndex("ActivityTypeId");

                    b.ToTable("ChallengeActivityTypes");
                });

            modelBuilder.Entity("SportClubsChallenges.Database.Entities.ChallengeParticipant", b =>
                {
                    b.Property<long>("ChallengeId")
                        .HasColumnType("bigint");

                    b.Property<long>("AthleteId")
                        .HasColumnType("bigint");

                    b.Property<DateTime>("LastUpdateDate")
                        .HasColumnType("datetime2");

                    b.Property<DateTime>("RegistrationDate")
                        .HasColumnType("datetime2");

                    b.Property<decimal>("Score")
                        .HasColumnType("decimal(18,2)");

                    b.HasKey("ChallengeId", "AthleteId");

                    b.HasIndex("AthleteId");

                    b.ToTable("ChallengeParticipants");
                });

            modelBuilder.Entity("SportClubsChallenges.Database.Entities.Club", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Description")
                        .HasColumnType("nvarchar(max)");

                    b.Property<byte[]>("Icon")
                        .HasColumnType("varbinary(max)");

                    b.Property<int>("MembersCount")
                        .HasColumnType("int");

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(max)");

                    b.Property<long>("OwnerId")
                        .HasColumnType("bigint");

                    b.Property<string>("SportType")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("OwnerId");

                    b.ToTable("Clubs");

                    b.HasData(
                        new
                        {
                            Id = 1L,
                            Description = "My first bike club",
                            MembersCount = 0,
                            Name = "Bike Club",
                            OwnerId = 1L,
                            SportType = "Bike"
                        },
                        new
                        {
                            Id = 2L,
                            Description = "Club for my running collages",
                            MembersCount = 0,
                            Name = "Club for runners",
                            OwnerId = 1L,
                            SportType = "Run"
                        });
                });

            modelBuilder.Entity("SportClubsChallenges.Database.Entities.Athlete", b =>
                {
                    b.HasOne("SportClubsChallenges.Database.Entities.AthleteStravaToken", "AthleteStravaToken")
                        .WithMany()
                        .HasForeignKey("AthleteStravaTokenId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("SportClubsChallenges.Database.Entities.Challenge", b =>
                {
                    b.HasOne("SportClubsChallenges.Database.Entities.Club", "Club")
                        .WithMany()
                        .HasForeignKey("ClubId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("SportClubsChallenges.Database.Entities.Athlete", "Owner")
                        .WithMany()
                        .HasForeignKey("OwnerId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();
                });

            modelBuilder.Entity("SportClubsChallenges.Database.Entities.ChallengeActivityType", b =>
                {
                    b.HasOne("SportClubsChallenges.Database.Entities.ActivityType", "ActivityType")
                        .WithMany()
                        .HasForeignKey("ActivityTypeId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("SportClubsChallenges.Database.Entities.Challenge", "Challenge")
                        .WithMany("ChallengeActivityTypes")
                        .HasForeignKey("ChallengeId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("SportClubsChallenges.Database.Entities.ChallengeParticipant", b =>
                {
                    b.HasOne("SportClubsChallenges.Database.Entities.Athlete", "Athlete")
                        .WithMany("ChallengeParticipants")
                        .HasForeignKey("AthleteId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("SportClubsChallenges.Database.Entities.Challenge", "Challenge")
                        .WithMany("ChallengeParticipants")
                        .HasForeignKey("ChallengeId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();
                });

            modelBuilder.Entity("SportClubsChallenges.Database.Entities.Club", b =>
                {
                    b.HasOne("SportClubsChallenges.Database.Entities.Athlete", "Owner")
                        .WithMany("OwnedClubs")
                        .HasForeignKey("OwnerId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });
#pragma warning restore 612, 618
        }
    }
}
