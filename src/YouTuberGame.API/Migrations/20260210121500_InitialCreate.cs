using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace YouTuberGame.API.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Characters",
                columns: table => new
                {
                    CharacterId = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CharacterName = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Rarity = table.Column<int>(type: "int", nullable: false),
                    Specialty = table.Column<int>(type: "int", nullable: false),
                    BaseFilming = table.Column<int>(type: "int", nullable: false),
                    BaseEditing = table.Column<int>(type: "int", nullable: false),
                    BasePlanning = table.Column<int>(type: "int", nullable: false),
                    BaseDesign = table.Column<int>(type: "int", nullable: false),
                    PassiveSkillDesc = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    PassiveSkillValue = table.Column<float>(type: "float", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Characters", x => x.CharacterId);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "varchar(255)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    PlayerName = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ChannelName = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Email = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    PasswordHash = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    LastLogin = table.Column<DateTime>(type: "datetime(6)", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.UserId);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "PlayerCharacters",
                columns: table => new
                {
                    InstanceId = table.Column<string>(type: "varchar(255)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    UserId = table.Column<string>(type: "varchar(255)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CharacterId = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Level = table.Column<int>(type: "int", nullable: false),
                    Experience = table.Column<int>(type: "int", nullable: false),
                    Breakthrough = table.Column<int>(type: "int", nullable: false),
                    AcquiredAt = table.Column<DateTime>(type: "datetime(6)", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlayerCharacters", x => x.InstanceId);
                    table.ForeignKey(
                        name: "FK_PlayerCharacters_Characters_CharacterId",
                        column: x => x.CharacterId,
                        principalTable: "Characters",
                        principalColumn: "CharacterId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PlayerCharacters_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "PlayerData",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "varchar(255)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Subscribers = table.Column<long>(type: "bigint", nullable: false),
                    TotalViews = table.Column<long>(type: "bigint", nullable: false),
                    ChannelPower = table.Column<long>(type: "bigint", nullable: false),
                    Gold = table.Column<long>(type: "bigint", nullable: false),
                    Gems = table.Column<int>(type: "int", nullable: false),
                    GachaTickets = table.Column<int>(type: "int", nullable: false),
                    ExpChips = table.Column<int>(type: "int", nullable: false),
                    StudioLevel = table.Column<int>(type: "int", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlayerData", x => x.UserId);
                    table.ForeignKey(
                        name: "FK_PlayerData_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.InsertData(
                table: "Characters",
                columns: new[] { "CharacterId", "BaseDesign", "BaseEditing", "BaseFilming", "BasePlanning", "CharacterName", "PassiveSkillDesc", "PassiveSkillValue", "Rarity", "Specialty" },
                values: new object[,]
                {
                    { "char_001", 10, 10, 30, 10, "김촬영", null, 0f, 0, 0 },
                    { "char_002", 10, 30, 10, 10, "박편집", null, 0f, 0, 1 },
                    { "char_003", 10, 10, 10, 30, "이기획", null, 0f, 0, 2 },
                    { "char_004", 30, 10, 10, 10, "최디자인", null, 0f, 0, 3 },
                    { "char_005", 15, 15, 15, 40, "강PD", null, 0f, 1, 2 },
                    { "char_006", 20, 30, 50, 20, "정프로", "촬영 품질 +10%", 0.1f, 2, 0 },
                    { "char_007", 40, 40, 40, 60, "천재 크리에이터", "전체 스탯 +15%", 0.15f, 3, 2 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_PlayerCharacters_CharacterId",
                table: "PlayerCharacters",
                column: "CharacterId");

            migrationBuilder.CreateIndex(
                name: "IX_PlayerCharacters_UserId",
                table: "PlayerCharacters",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_Email",
                table: "Users",
                column: "Email",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PlayerCharacters");

            migrationBuilder.DropTable(
                name: "PlayerData");

            migrationBuilder.DropTable(
                name: "Characters");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
