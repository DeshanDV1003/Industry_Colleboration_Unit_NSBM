using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NSBM.ICU.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class EmailNotifications : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "EmailHandledAt",
                table: "Notifications",
                type: "datetime2",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "EmailPreferences",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: false),
                    NotificationsByEmail = table.Column<bool>(type: "bit", nullable: false),
                    WeeklyDigestByEmail = table.Column<bool>(type: "bit", nullable: false),
                    LastDigestSentAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmailPreferences", x => x.UserId);
                    table.ForeignKey(
                        name: "FK_EmailPreferences_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_EmailHandledAt_CreatedAt",
                table: "Notifications",
                columns: new[] { "EmailHandledAt", "CreatedAt" });

            // Notifications that already existed were seen in the app; do not e-mail that backlog on first start.
            migrationBuilder.Sql("UPDATE Notifications SET EmailHandledAt = SYSUTCDATETIME() WHERE EmailHandledAt IS NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EmailPreferences");

            migrationBuilder.DropIndex(
                name: "IX_Notifications_EmailHandledAt_CreatedAt",
                table: "Notifications");

            migrationBuilder.DropColumn(
                name: "EmailHandledAt",
                table: "Notifications");
        }
    }
}
