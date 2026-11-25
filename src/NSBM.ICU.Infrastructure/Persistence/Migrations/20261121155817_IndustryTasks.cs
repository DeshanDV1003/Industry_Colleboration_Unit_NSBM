using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NSBM.ICU.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class IndustryTasks : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "IndustryTasks",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CompanyId = table.Column<int>(type: "int", nullable: false),
                    AuthorPartnerId = table.Column<int>(type: "int", nullable: false),
                    CareerPathId = table.Column<int>(type: "int", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Summary = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Instructions = table.Column<string>(type: "nvarchar(max)", maxLength: 8000, nullable: false),
                    Level = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Situation = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    EstimatedMinutes = table.Column<int>(type: "int", nullable: false),
                    StarterCode = table.Column<string>(type: "nvarchar(max)", maxLength: 20000, nullable: true),
                    StarterLanguage = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: true),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    ReviewedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ReviewedBy = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    ReviewNote = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IndustryTasks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_IndustryTasks_CareerPaths_CareerPathId",
                        column: x => x.CareerPathId,
                        principalTable: "CareerPaths",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_IndustryTasks_Companies_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Companies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_IndustryTasks_IndustryPartners_AuthorPartnerId",
                        column: x => x.AuthorPartnerId,
                        principalTable: "IndustryPartners",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "KnowledgeCheckSubmissions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StudentId = table.Column<int>(type: "int", nullable: false),
                    TaskId = table.Column<int>(type: "int", nullable: false),
                    SubmittedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_KnowledgeCheckSubmissions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_KnowledgeCheckSubmissions_IndustryTasks_TaskId",
                        column: x => x.TaskId,
                        principalTable: "IndustryTasks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_KnowledgeCheckSubmissions_Students_StudentId",
                        column: x => x.StudentId,
                        principalTable: "Students",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "KnowledgeQuestion",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TaskId = table.Column<int>(type: "int", nullable: false),
                    SortOrder = table.Column<int>(type: "int", nullable: false),
                    Prompt = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_KnowledgeQuestion", x => x.Id);
                    table.ForeignKey(
                        name: "FK_KnowledgeQuestion_IndustryTasks_TaskId",
                        column: x => x.TaskId,
                        principalTable: "IndustryTasks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TaskAttempts",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StudentId = table.Column<int>(type: "int", nullable: false),
                    TaskId = table.Column<int>(type: "int", nullable: false),
                    AttemptNumber = table.Column<int>(type: "int", nullable: false),
                    Code = table.Column<string>(type: "nvarchar(max)", maxLength: 20000, nullable: false),
                    SubmittedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    PassedChecks = table.Column<int>(type: "int", nullable: false),
                    TotalChecks = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TaskAttempts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TaskAttempts_IndustryTasks_TaskId",
                        column: x => x.TaskId,
                        principalTable: "IndustryTasks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TaskAttempts_Students_StudentId",
                        column: x => x.StudentId,
                        principalTable: "Students",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TaskCheck",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TaskId = table.Column<int>(type: "int", nullable: false),
                    SortOrder = table.Column<int>(type: "int", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false),
                    Pattern = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    ShouldMatch = table.Column<bool>(type: "bit", nullable: false),
                    Hint = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TaskCheck", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TaskCheck_IndustryTasks_TaskId",
                        column: x => x.TaskId,
                        principalTable: "IndustryTasks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TaskSkill",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TaskId = table.Column<int>(type: "int", nullable: false),
                    SkillId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TaskSkill", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TaskSkill_IndustryTasks_TaskId",
                        column: x => x.TaskId,
                        principalTable: "IndustryTasks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TaskSkill_Skills_SkillId",
                        column: x => x.SkillId,
                        principalTable: "Skills",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "KnowledgeAnswer",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SubmissionId = table.Column<int>(type: "int", nullable: false),
                    QuestionId = table.Column<int>(type: "int", nullable: false),
                    Text = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_KnowledgeAnswer", x => x.Id);
                    table.ForeignKey(
                        name: "FK_KnowledgeAnswer_KnowledgeCheckSubmissions_SubmissionId",
                        column: x => x.SubmissionId,
                        principalTable: "KnowledgeCheckSubmissions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_KnowledgeAnswer_KnowledgeQuestion_QuestionId",
                        column: x => x.QuestionId,
                        principalTable: "KnowledgeQuestion",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TaskCheckResult",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AttemptId = table.Column<int>(type: "int", nullable: false),
                    CheckId = table.Column<int>(type: "int", nullable: false),
                    Passed = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TaskCheckResult", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TaskCheckResult_TaskAttempts_AttemptId",
                        column: x => x.AttemptId,
                        principalTable: "TaskAttempts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TaskCheckResult_TaskCheck_CheckId",
                        column: x => x.CheckId,
                        principalTable: "TaskCheck",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_IndustryTasks_AuthorPartnerId",
                table: "IndustryTasks",
                column: "AuthorPartnerId");

            migrationBuilder.CreateIndex(
                name: "IX_IndustryTasks_CareerPathId",
                table: "IndustryTasks",
                column: "CareerPathId");

            migrationBuilder.CreateIndex(
                name: "IX_IndustryTasks_CompanyId",
                table: "IndustryTasks",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_IndustryTasks_Status_CareerPathId",
                table: "IndustryTasks",
                columns: new[] { "Status", "CareerPathId" });

            migrationBuilder.CreateIndex(
                name: "IX_KnowledgeAnswer_QuestionId",
                table: "KnowledgeAnswer",
                column: "QuestionId");

            migrationBuilder.CreateIndex(
                name: "IX_KnowledgeAnswer_SubmissionId_QuestionId",
                table: "KnowledgeAnswer",
                columns: new[] { "SubmissionId", "QuestionId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_KnowledgeCheckSubmissions_StudentId_TaskId",
                table: "KnowledgeCheckSubmissions",
                columns: new[] { "StudentId", "TaskId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_KnowledgeCheckSubmissions_TaskId",
                table: "KnowledgeCheckSubmissions",
                column: "TaskId");

            migrationBuilder.CreateIndex(
                name: "IX_KnowledgeQuestion_TaskId",
                table: "KnowledgeQuestion",
                column: "TaskId");

            migrationBuilder.CreateIndex(
                name: "IX_TaskAttempts_StudentId_TaskId_AttemptNumber",
                table: "TaskAttempts",
                columns: new[] { "StudentId", "TaskId", "AttemptNumber" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TaskAttempts_TaskId",
                table: "TaskAttempts",
                column: "TaskId");

            migrationBuilder.CreateIndex(
                name: "IX_TaskCheck_TaskId",
                table: "TaskCheck",
                column: "TaskId");

            migrationBuilder.CreateIndex(
                name: "IX_TaskCheckResult_AttemptId_CheckId",
                table: "TaskCheckResult",
                columns: new[] { "AttemptId", "CheckId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TaskCheckResult_CheckId",
                table: "TaskCheckResult",
                column: "CheckId");

            migrationBuilder.CreateIndex(
                name: "IX_TaskSkill_SkillId",
                table: "TaskSkill",
                column: "SkillId");

            migrationBuilder.CreateIndex(
                name: "IX_TaskSkill_TaskId_SkillId",
                table: "TaskSkill",
                columns: new[] { "TaskId", "SkillId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "KnowledgeAnswer");

            migrationBuilder.DropTable(
                name: "TaskCheckResult");

            migrationBuilder.DropTable(
                name: "TaskSkill");

            migrationBuilder.DropTable(
                name: "KnowledgeCheckSubmissions");

            migrationBuilder.DropTable(
                name: "KnowledgeQuestion");

            migrationBuilder.DropTable(
                name: "TaskAttempts");

            migrationBuilder.DropTable(
                name: "TaskCheck");

            migrationBuilder.DropTable(
                name: "IndustryTasks");
        }
    }
}
