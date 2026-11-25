using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NSBM.ICU.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class Interviews : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "InterviewQuestions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Stage = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    CareerPathId = table.Column<int>(type: "int", nullable: true),
                    ParentQuestionId = table.Column<int>(type: "int", nullable: true),
                    SortOrder = table.Column<int>(type: "int", nullable: false),
                    Topic = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Prompt = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Keywords = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Tip = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InterviewQuestions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_InterviewQuestions_CareerPaths_CareerPathId",
                        column: x => x.CareerPathId,
                        principalTable: "CareerPaths",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_InterviewQuestions_InterviewQuestions_ParentQuestionId",
                        column: x => x.ParentQuestionId,
                        principalTable: "InterviewQuestions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "InterviewAnswers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StudentId = table.Column<int>(type: "int", nullable: false),
                    QuestionId = table.Column<int>(type: "int", nullable: false),
                    Text = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: false),
                    InputMode = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    AnsweredAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    MatchedKeywords = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    WordCount = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InterviewAnswers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_InterviewAnswers_InterviewQuestions_QuestionId",
                        column: x => x.QuestionId,
                        principalTable: "InterviewQuestions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_InterviewAnswers_Students_StudentId",
                        column: x => x.StudentId,
                        principalTable: "Students",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_InterviewAnswers_QuestionId",
                table: "InterviewAnswers",
                column: "QuestionId");

            migrationBuilder.CreateIndex(
                name: "IX_InterviewAnswers_StudentId_QuestionId_AnsweredAt",
                table: "InterviewAnswers",
                columns: new[] { "StudentId", "QuestionId", "AnsweredAt" });

            migrationBuilder.CreateIndex(
                name: "IX_InterviewQuestions_CareerPathId",
                table: "InterviewQuestions",
                column: "CareerPathId");

            migrationBuilder.CreateIndex(
                name: "IX_InterviewQuestions_ParentQuestionId",
                table: "InterviewQuestions",
                column: "ParentQuestionId");

            migrationBuilder.CreateIndex(
                name: "IX_InterviewQuestions_Stage_CareerPathId",
                table: "InterviewQuestions",
                columns: new[] { "Stage", "CareerPathId" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "InterviewAnswers");

            migrationBuilder.DropTable(
                name: "InterviewQuestions");
        }
    }
}
