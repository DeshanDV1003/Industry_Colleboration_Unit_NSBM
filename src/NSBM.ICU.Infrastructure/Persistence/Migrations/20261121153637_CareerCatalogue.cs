using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NSBM.ICU.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class CareerCatalogue : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CareerPathId",
                table: "Students",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CareerPathSelectedAt",
                table: "Students",
                type: "datetime2",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "CareerPaths",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Slug = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Tagline = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: false),
                    EntryRoles = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    SortOrder = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CareerPaths", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Skills",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Category = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Skills", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "BaselineAttempts",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StudentId = table.Column<int>(type: "int", nullable: false),
                    CareerPathId = table.Column<int>(type: "int", nullable: false),
                    CompletedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CorrectCount = table.Column<int>(type: "int", nullable: false),
                    QuestionCount = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BaselineAttempts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BaselineAttempts_CareerPaths_CareerPathId",
                        column: x => x.CareerPathId,
                        principalTable: "CareerPaths",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_BaselineAttempts_Students_StudentId",
                        column: x => x.StudentId,
                        principalTable: "Students",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "BaselineQuestions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CareerPathId = table.Column<int>(type: "int", nullable: false),
                    SkillId = table.Column<int>(type: "int", nullable: false),
                    SortOrder = table.Column<int>(type: "int", nullable: false),
                    Prompt = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Explanation = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BaselineQuestions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BaselineQuestions_CareerPaths_CareerPathId",
                        column: x => x.CareerPathId,
                        principalTable: "CareerPaths",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BaselineQuestions_Skills_SkillId",
                        column: x => x.SkillId,
                        principalTable: "Skills",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CareerPathSkills",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CareerPathId = table.Column<int>(type: "int", nullable: false),
                    SkillId = table.Column<int>(type: "int", nullable: false),
                    Importance = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CareerPathSkills", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CareerPathSkills_CareerPaths_CareerPathId",
                        column: x => x.CareerPathId,
                        principalTable: "CareerPaths",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CareerPathSkills_Skills_SkillId",
                        column: x => x.SkillId,
                        principalTable: "Skills",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "RoadmapSteps",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CareerPathId = table.Column<int>(type: "int", nullable: false),
                    SortOrder = table.Column<int>(type: "int", nullable: false),
                    Stage = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Title = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    Outcome = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    SkillId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RoadmapSteps", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RoadmapSteps_CareerPaths_CareerPathId",
                        column: x => x.CareerPathId,
                        principalTable: "CareerPaths",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RoadmapSteps_Skills_SkillId",
                        column: x => x.SkillId,
                        principalTable: "Skills",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "BaselineOption",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    QuestionId = table.Column<int>(type: "int", nullable: false),
                    SortOrder = table.Column<int>(type: "int", nullable: false),
                    Text = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false),
                    IsCorrect = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BaselineOption", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BaselineOption_BaselineQuestions_QuestionId",
                        column: x => x.QuestionId,
                        principalTable: "BaselineQuestions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RoadmapStepCompletions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StudentId = table.Column<int>(type: "int", nullable: false),
                    RoadmapStepId = table.Column<int>(type: "int", nullable: false),
                    CompletedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RoadmapStepCompletions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RoadmapStepCompletions_RoadmapSteps_RoadmapStepId",
                        column: x => x.RoadmapStepId,
                        principalTable: "RoadmapSteps",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RoadmapStepCompletions_Students_StudentId",
                        column: x => x.StudentId,
                        principalTable: "Students",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "BaselineAnswer",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AttemptId = table.Column<int>(type: "int", nullable: false),
                    QuestionId = table.Column<int>(type: "int", nullable: false),
                    SelectedOptionId = table.Column<int>(type: "int", nullable: false),
                    IsCorrect = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BaselineAnswer", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BaselineAnswer_BaselineAttempts_AttemptId",
                        column: x => x.AttemptId,
                        principalTable: "BaselineAttempts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BaselineAnswer_BaselineOption_SelectedOptionId",
                        column: x => x.SelectedOptionId,
                        principalTable: "BaselineOption",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_BaselineAnswer_BaselineQuestions_QuestionId",
                        column: x => x.QuestionId,
                        principalTable: "BaselineQuestions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Students_CareerPathId",
                table: "Students",
                column: "CareerPathId");

            migrationBuilder.CreateIndex(
                name: "IX_BaselineAnswer_AttemptId_QuestionId",
                table: "BaselineAnswer",
                columns: new[] { "AttemptId", "QuestionId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_BaselineAnswer_QuestionId",
                table: "BaselineAnswer",
                column: "QuestionId");

            migrationBuilder.CreateIndex(
                name: "IX_BaselineAnswer_SelectedOptionId",
                table: "BaselineAnswer",
                column: "SelectedOptionId");

            migrationBuilder.CreateIndex(
                name: "IX_BaselineAttempts_CareerPathId",
                table: "BaselineAttempts",
                column: "CareerPathId");

            migrationBuilder.CreateIndex(
                name: "IX_BaselineAttempts_StudentId_CareerPathId_CompletedAt",
                table: "BaselineAttempts",
                columns: new[] { "StudentId", "CareerPathId", "CompletedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_BaselineOption_QuestionId",
                table: "BaselineOption",
                column: "QuestionId");

            migrationBuilder.CreateIndex(
                name: "IX_BaselineQuestions_CareerPathId",
                table: "BaselineQuestions",
                column: "CareerPathId");

            migrationBuilder.CreateIndex(
                name: "IX_BaselineQuestions_SkillId",
                table: "BaselineQuestions",
                column: "SkillId");

            migrationBuilder.CreateIndex(
                name: "IX_CareerPaths_Slug",
                table: "CareerPaths",
                column: "Slug",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CareerPathSkills_CareerPathId_SkillId",
                table: "CareerPathSkills",
                columns: new[] { "CareerPathId", "SkillId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CareerPathSkills_SkillId",
                table: "CareerPathSkills",
                column: "SkillId");

            migrationBuilder.CreateIndex(
                name: "IX_RoadmapStepCompletions_RoadmapStepId",
                table: "RoadmapStepCompletions",
                column: "RoadmapStepId");

            migrationBuilder.CreateIndex(
                name: "IX_RoadmapStepCompletions_StudentId_RoadmapStepId",
                table: "RoadmapStepCompletions",
                columns: new[] { "StudentId", "RoadmapStepId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RoadmapSteps_CareerPathId_SortOrder",
                table: "RoadmapSteps",
                columns: new[] { "CareerPathId", "SortOrder" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RoadmapSteps_SkillId",
                table: "RoadmapSteps",
                column: "SkillId");

            migrationBuilder.CreateIndex(
                name: "IX_Skills_Name",
                table: "Skills",
                column: "Name",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Students_CareerPaths_CareerPathId",
                table: "Students",
                column: "CareerPathId",
                principalTable: "CareerPaths",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Students_CareerPaths_CareerPathId",
                table: "Students");

            migrationBuilder.DropTable(
                name: "BaselineAnswer");

            migrationBuilder.DropTable(
                name: "CareerPathSkills");

            migrationBuilder.DropTable(
                name: "RoadmapStepCompletions");

            migrationBuilder.DropTable(
                name: "BaselineAttempts");

            migrationBuilder.DropTable(
                name: "BaselineOption");

            migrationBuilder.DropTable(
                name: "RoadmapSteps");

            migrationBuilder.DropTable(
                name: "BaselineQuestions");

            migrationBuilder.DropTable(
                name: "CareerPaths");

            migrationBuilder.DropTable(
                name: "Skills");

            migrationBuilder.DropIndex(
                name: "IX_Students_CareerPathId",
                table: "Students");

            migrationBuilder.DropColumn(
                name: "CareerPathId",
                table: "Students");

            migrationBuilder.DropColumn(
                name: "CareerPathSelectedAt",
                table: "Students");
        }
    }
}
