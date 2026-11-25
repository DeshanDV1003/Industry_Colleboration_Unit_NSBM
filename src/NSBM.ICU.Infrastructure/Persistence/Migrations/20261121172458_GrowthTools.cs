using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NSBM.ICU.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class GrowthTools : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CvProfiles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StudentId = table.Column<int>(type: "int", nullable: false),
                    Headline = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Summary = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: false),
                    Education = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: false),
                    Projects = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: false),
                    GitHubUrl = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: true),
                    LinkedInUrl = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CvProfiles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CvProfiles_Students_StudentId",
                        column: x => x.StudentId,
                        principalTable: "Students",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MentorQuestions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StudentId = table.Column<int>(type: "int", nullable: false),
                    CareerPathId = table.Column<int>(type: "int", nullable: true),
                    Title = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Body = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: false),
                    AskedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Reply = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    RepliedBy = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    RepliedByCompany = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    RepliedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MentorQuestions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MentorQuestions_CareerPaths_CareerPathId",
                        column: x => x.CareerPathId,
                        principalTable: "CareerPaths",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_MentorQuestions_Students_StudentId",
                        column: x => x.StudentId,
                        principalTable: "Students",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PlannerItems",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StudentId = table.Column<int>(type: "int", nullable: false),
                    Kind = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Title = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    DueDate = table.Column<DateOnly>(type: "date", nullable: true),
                    IsDone = table.Column<bool>(type: "bit", nullable: false),
                    CompletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlannerItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PlannerItems_Students_StudentId",
                        column: x => x.StudentId,
                        principalTable: "Students",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SprintWorkItems",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Key = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    CareerPathId = table.Column<int>(type: "int", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    StoryPoints = table.Column<int>(type: "int", nullable: false),
                    TeammateName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    TeammateStatus = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    SortOrder = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SprintWorkItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SprintWorkItems_CareerPaths_CareerPathId",
                        column: x => x.CareerPathId,
                        principalTable: "CareerPaths",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TaskAttemptReviews",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AttemptId = table.Column<int>(type: "int", nullable: false),
                    ReviewerPartnerId = table.Column<int>(type: "int", nullable: false),
                    ReviewerName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Comment = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: false),
                    Improvements = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    ReviewedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TaskAttemptReviews", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TaskAttemptReviews_IndustryPartners_ReviewerPartnerId",
                        column: x => x.ReviewerPartnerId,
                        principalTable: "IndustryPartners",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TaskAttemptReviews_TaskAttempts_AttemptId",
                        column: x => x.AttemptId,
                        principalTable: "TaskAttempts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SprintItemProgress",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StudentId = table.Column<int>(type: "int", nullable: false),
                    WorkItemId = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SprintItemProgress", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SprintItemProgress_SprintWorkItems_WorkItemId",
                        column: x => x.WorkItemId,
                        principalTable: "SprintWorkItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SprintItemProgress_Students_StudentId",
                        column: x => x.StudentId,
                        principalTable: "Students",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CvProfiles_StudentId",
                table: "CvProfiles",
                column: "StudentId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_MentorQuestions_CareerPathId",
                table: "MentorQuestions",
                column: "CareerPathId");

            migrationBuilder.CreateIndex(
                name: "IX_MentorQuestions_StudentId_AskedAt",
                table: "MentorQuestions",
                columns: new[] { "StudentId", "AskedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_PlannerItems_StudentId_IsDone_DueDate",
                table: "PlannerItems",
                columns: new[] { "StudentId", "IsDone", "DueDate" });

            migrationBuilder.CreateIndex(
                name: "IX_SprintItemProgress_StudentId_WorkItemId",
                table: "SprintItemProgress",
                columns: new[] { "StudentId", "WorkItemId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SprintItemProgress_WorkItemId",
                table: "SprintItemProgress",
                column: "WorkItemId");

            migrationBuilder.CreateIndex(
                name: "IX_SprintWorkItems_CareerPathId",
                table: "SprintWorkItems",
                column: "CareerPathId");

            migrationBuilder.CreateIndex(
                name: "IX_SprintWorkItems_Key",
                table: "SprintWorkItems",
                column: "Key",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TaskAttemptReviews_AttemptId",
                table: "TaskAttemptReviews",
                column: "AttemptId");

            migrationBuilder.CreateIndex(
                name: "IX_TaskAttemptReviews_ReviewerPartnerId",
                table: "TaskAttemptReviews",
                column: "ReviewerPartnerId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CvProfiles");

            migrationBuilder.DropTable(
                name: "MentorQuestions");

            migrationBuilder.DropTable(
                name: "PlannerItems");

            migrationBuilder.DropTable(
                name: "SprintItemProgress");

            migrationBuilder.DropTable(
                name: "TaskAttemptReviews");

            migrationBuilder.DropTable(
                name: "SprintWorkItems");
        }
    }
}
