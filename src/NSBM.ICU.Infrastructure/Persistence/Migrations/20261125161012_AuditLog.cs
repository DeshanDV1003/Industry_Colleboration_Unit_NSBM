using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NSBM.ICU.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AuditLog : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AuditEntries",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    At = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ActorUserId = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: true),
                    ActorName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    ActorRole = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    Category = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Action = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: false),
                    Summary = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    SubjectType = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: true),
                    SubjectId = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AuditEntries", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AuditEntries_At",
                table: "AuditEntries",
                column: "At");

            migrationBuilder.CreateIndex(
                name: "IX_AuditEntries_Category_At",
                table: "AuditEntries",
                columns: new[] { "Category", "At" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AuditEntries");
        }
    }
}
