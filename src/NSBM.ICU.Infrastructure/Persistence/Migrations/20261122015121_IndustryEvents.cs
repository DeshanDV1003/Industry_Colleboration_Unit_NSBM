using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NSBM.ICU.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class IndustryEvents : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "IndustryEvents",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: false),
                    Format = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    StartDate = table.Column<DateOnly>(type: "date", nullable: false),
                    EndDate = table.Column<DateOnly>(type: "date", nullable: false),
                    Location = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Capacity = table.Column<int>(type: "int", nullable: false),
                    RegistrationClosesOn = table.Column<DateOnly>(type: "date", nullable: false),
                    MinTeamSize = table.Column<int>(type: "int", nullable: true),
                    MaxTeamSize = table.Column<int>(type: "int", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CompanyId = table.Column<int>(type: "int", nullable: false),
                    AuthorPartnerId = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    ReviewedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ReviewedBy = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    ReviewNote = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IndustryEvents", x => x.Id);
                    table.ForeignKey(
                        name: "FK_IndustryEvents_Companies_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Companies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_IndustryEvents_IndustryPartners_AuthorPartnerId",
                        column: x => x.AuthorPartnerId,
                        principalTable: "IndustryPartners",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "EventTeams",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EventId = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: false),
                    JoinCode = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    CaptainStudentId = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EventTeams", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EventTeams_IndustryEvents_EventId",
                        column: x => x.EventId,
                        principalTable: "IndustryEvents",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "EventRegistrations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EventId = table.Column<int>(type: "int", nullable: false),
                    StudentId = table.Column<int>(type: "int", nullable: false),
                    RegisteredAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TeamId = table.Column<int>(type: "int", nullable: true),
                    Attended = table.Column<bool>(type: "bit", nullable: false),
                    CertificateCode = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    CertificateIssuedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EventRegistrations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EventRegistrations_EventTeams_TeamId",
                        column: x => x.TeamId,
                        principalTable: "EventTeams",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_EventRegistrations_IndustryEvents_EventId",
                        column: x => x.EventId,
                        principalTable: "IndustryEvents",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EventRegistrations_Students_StudentId",
                        column: x => x.StudentId,
                        principalTable: "Students",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_EventRegistrations_CertificateCode",
                table: "EventRegistrations",
                column: "CertificateCode",
                unique: true,
                filter: "[CertificateCode] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_EventRegistrations_EventId_StudentId",
                table: "EventRegistrations",
                columns: new[] { "EventId", "StudentId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_EventRegistrations_StudentId",
                table: "EventRegistrations",
                column: "StudentId");

            migrationBuilder.CreateIndex(
                name: "IX_EventRegistrations_TeamId",
                table: "EventRegistrations",
                column: "TeamId");

            migrationBuilder.CreateIndex(
                name: "IX_EventTeams_EventId_JoinCode",
                table: "EventTeams",
                columns: new[] { "EventId", "JoinCode" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_IndustryEvents_AuthorPartnerId",
                table: "IndustryEvents",
                column: "AuthorPartnerId");

            migrationBuilder.CreateIndex(
                name: "IX_IndustryEvents_CompanyId",
                table: "IndustryEvents",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_IndustryEvents_Status_StartDate",
                table: "IndustryEvents",
                columns: new[] { "Status", "StartDate" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EventRegistrations");

            migrationBuilder.DropTable(
                name: "EventTeams");

            migrationBuilder.DropTable(
                name: "IndustryEvents");
        }
    }
}
