using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace SportClubsChallenges.Database.Migrations
{
    public partial class initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ActivityTypes",
                columns: table => new
                {
                    Id = table.Column<byte>(nullable: false),
                    Name = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ActivityTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AthleteStravaTokens",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AccessToken = table.Column<string>(nullable: true),
                    RefreshToken = table.Column<string>(nullable: true),
                    TokenType = table.Column<string>(nullable: true),
                    ExpirationDate = table.Column<DateTimeOffset>(nullable: false),
                    LastUpdateDate = table.Column<DateTimeOffset>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AthleteStravaTokens", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Athletes",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false),
                    FirstName = table.Column<string>(nullable: true),
                    LastName = table.Column<string>(nullable: true),
                    IconUrlLarge = table.Column<string>(nullable: true),
                    IconUrlMedium = table.Column<string>(nullable: true),
                    City = table.Column<string>(nullable: true),
                    Gender = table.Column<string>(nullable: true),
                    Country = table.Column<string>(nullable: true),
                    CreationDate = table.Column<DateTime>(nullable: false),
                    LastLoginDate = table.Column<DateTime>(nullable: false),
                    AthleteStravaTokenId = table.Column<long>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Athletes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Athletes_AthleteStravaTokens_AthleteStravaTokenId",
                        column: x => x.AthleteStravaTokenId,
                        principalTable: "AthleteStravaTokens",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Clubs",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(nullable: true),
                    Description = table.Column<string>(nullable: true),
                    SportType = table.Column<string>(nullable: true),
                    Icon = table.Column<byte[]>(nullable: true),
                    OwnerId = table.Column<long>(nullable: false),
                    MembersCount = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Clubs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Clubs_Athletes_OwnerId",
                        column: x => x.OwnerId,
                        principalTable: "Athletes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Challenges",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(nullable: true),
                    Description = table.Column<string>(nullable: true),
                    IsActive = table.Column<bool>(nullable: false),
                    ClubId = table.Column<long>(nullable: false),
                    StartDate = table.Column<DateTime>(nullable: false),
                    EndDate = table.Column<DateTime>(nullable: false),
                    ChallengeType = table.Column<byte>(nullable: false),
                    PreventManualActivities = table.Column<bool>(nullable: false),
                    IncludeOnlyGpsActivities = table.Column<bool>(nullable: false),
                    OwnerId = table.Column<long>(nullable: false),
                    CreationDate = table.Column<DateTime>(nullable: false),
                    EditionDate = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Challenges", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Challenges_Clubs_ClubId",
                        column: x => x.ClubId,
                        principalTable: "Clubs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Challenges_Athletes_OwnerId",
                        column: x => x.OwnerId,
                        principalTable: "Athletes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ChallengeActivityTypes",
                columns: table => new
                {
                    ChallengeId = table.Column<long>(nullable: false),
                    ActivityTypeId = table.Column<byte>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChallengeActivityTypes", x => new { x.ChallengeId, x.ActivityTypeId });
                    table.ForeignKey(
                        name: "FK_ChallengeActivityTypes_ActivityTypes_ActivityTypeId",
                        column: x => x.ActivityTypeId,
                        principalTable: "ActivityTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ChallengeActivityTypes_Challenges_ChallengeId",
                        column: x => x.ChallengeId,
                        principalTable: "Challenges",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ChallengeParticipants",
                columns: table => new
                {
                    ChallengeId = table.Column<long>(nullable: false),
                    AthleteId = table.Column<long>(nullable: false),
                    Score = table.Column<decimal>(nullable: false),
                    RegistrationDate = table.Column<DateTime>(nullable: false),
                    LastUpdateDate = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChallengeParticipants", x => new { x.ChallengeId, x.AthleteId });
                    table.ForeignKey(
                        name: "FK_ChallengeParticipants_Athletes_AthleteId",
                        column: x => x.AthleteId,
                        principalTable: "Athletes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ChallengeParticipants_Challenges_ChallengeId",
                        column: x => x.ChallengeId,
                        principalTable: "Challenges",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.InsertData(
                table: "ActivityTypes",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { (byte)1, "Run" },
                    { (byte)2, "Ride" },
                    { (byte)3, "Swim" },
                    { (byte)4, "VirtualRide" },
                    { (byte)5, "Walk" },
                    { (byte)6, "Other" }
                });

            migrationBuilder.InsertData(
                table: "AthleteStravaTokens",
                columns: new[] { "Id", "AccessToken", "ExpirationDate", "LastUpdateDate", "RefreshToken", "TokenType" },
                values: new object[] { 1L, "00000000-0000-0000-0000-000000000000", new DateTimeOffset(new DateTime(2021, 1, 31, 18, 41, 7, 53, DateTimeKind.Unspecified).AddTicks(2776), new TimeSpan(0, 1, 0, 0, 0)), new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "00000000-0000-0000-0000-000000000000", null });

            migrationBuilder.InsertData(
                table: "Athletes",
                columns: new[] { "Id", "AthleteStravaTokenId", "City", "Country", "CreationDate", "FirstName", "Gender", "IconUrlLarge", "IconUrlMedium", "LastLoginDate", "LastName" },
                values: new object[] { 1L, 1L, null, null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "John", null, null, null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Smith" });

            migrationBuilder.InsertData(
                table: "Clubs",
                columns: new[] { "Id", "Description", "Icon", "MembersCount", "Name", "OwnerId", "SportType" },
                values: new object[] { 1L, "My first bike club", null, 0, "Bike Club", 1L, "Bike" });

            migrationBuilder.InsertData(
                table: "Clubs",
                columns: new[] { "Id", "Description", "Icon", "MembersCount", "Name", "OwnerId", "SportType" },
                values: new object[] { 2L, "Club for my running collages", null, 0, "Club for runners", 1L, "Run" });

            migrationBuilder.CreateIndex(
                name: "IX_Athletes_AthleteStravaTokenId",
                table: "Athletes",
                column: "AthleteStravaTokenId");

            migrationBuilder.CreateIndex(
                name: "IX_ChallengeActivityTypes_ActivityTypeId",
                table: "ChallengeActivityTypes",
                column: "ActivityTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_ChallengeParticipants_AthleteId",
                table: "ChallengeParticipants",
                column: "AthleteId");

            migrationBuilder.CreateIndex(
                name: "IX_Challenges_ClubId",
                table: "Challenges",
                column: "ClubId");

            migrationBuilder.CreateIndex(
                name: "IX_Challenges_OwnerId",
                table: "Challenges",
                column: "OwnerId");

            migrationBuilder.CreateIndex(
                name: "IX_Clubs_OwnerId",
                table: "Clubs",
                column: "OwnerId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ChallengeActivityTypes");

            migrationBuilder.DropTable(
                name: "ChallengeParticipants");

            migrationBuilder.DropTable(
                name: "ActivityTypes");

            migrationBuilder.DropTable(
                name: "Challenges");

            migrationBuilder.DropTable(
                name: "Clubs");

            migrationBuilder.DropTable(
                name: "Athletes");

            migrationBuilder.DropTable(
                name: "AthleteStravaTokens");
        }
    }
}
