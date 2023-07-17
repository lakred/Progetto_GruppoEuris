using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ConcordiaDBLibrary.Migrations
{
    /// <inheritdoc />
    public partial class InitialMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Priorities",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false),
                    Color = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false),
                    Code = table.Column<string>(type: "varchar(24)", maxLength: 24, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Priorities", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Scientists",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FullName = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false),
                    Code = table.Column<string>(type: "varchar(24)", maxLength: 24, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Scientists", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "States",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false),
                    Code = table.Column<string>(type: "varchar(24)", maxLength: 24, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_States", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Experiments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "varchar(MAX)", maxLength: 2147483647, nullable: false),
                    Loaded = table.Column<bool>(type: "bit", nullable: false),
                    StartDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    DueDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    PriorityId = table.Column<int>(type: "int", nullable: false),
                    StateId = table.Column<int>(type: "int", nullable: false),
                    Code = table.Column<string>(type: "varchar(24)", maxLength: 24, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Experiments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Experiments_Priorities",
                        column: x => x.PriorityId,
                        principalTable: "Priorities",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Experiments_States",
                        column: x => x.StateId,
                        principalTable: "States",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Participants",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ExperimentId = table.Column<int>(type: "int", nullable: false),
                    ScientistId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Participants", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Participants_Experiments",
                        column: x => x.ExperimentId,
                        principalTable: "Experiments",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Participants_Scientists",
                        column: x => x.ScientistId,
                        principalTable: "Scientists",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Remarks",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Text = table.Column<string>(type: "varchar(MAX)", maxLength: 2147483647, nullable: false),
                    Date = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    ExperimentId = table.Column<int>(type: "int", nullable: false),
                    ScientistId = table.Column<int>(type: "int", nullable: false),
                    Code = table.Column<string>(type: "varchar(24)", maxLength: 24, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Remarks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Remarks_Experiments",
                        column: x => x.ExperimentId,
                        principalTable: "Experiments",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Remarks_Scientists",
                        column: x => x.ScientistId,
                        principalTable: "Scientists",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Experiments_PriorityId",
                table: "Experiments",
                column: "PriorityId");

            migrationBuilder.CreateIndex(
                name: "IX_Experiments_StateId",
                table: "Experiments",
                column: "StateId");

            migrationBuilder.CreateIndex(
                name: "IX_Participants_ExperimentId",
                table: "Participants",
                column: "ExperimentId");

            migrationBuilder.CreateIndex(
                name: "IX_Participants_ScientistId",
                table: "Participants",
                column: "ScientistId");

            migrationBuilder.CreateIndex(
                name: "IX_Remarks_ExperimentId",
                table: "Remarks",
                column: "ExperimentId");

            migrationBuilder.CreateIndex(
                name: "IX_Remarks_ScientistId",
                table: "Remarks",
                column: "ScientistId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Participants");

            migrationBuilder.DropTable(
                name: "Remarks");

            migrationBuilder.DropTable(
                name: "Experiments");

            migrationBuilder.DropTable(
                name: "Scientists");

            migrationBuilder.DropTable(
                name: "Priorities");

            migrationBuilder.DropTable(
                name: "States");
        }
    }
}
