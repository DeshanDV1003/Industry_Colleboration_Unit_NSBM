using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NSBM.ICU.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class PluralTableNames : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BaselineAnswer_BaselineAttempts_AttemptId",
                table: "BaselineAnswer");

            migrationBuilder.DropForeignKey(
                name: "FK_BaselineAnswer_BaselineOption_SelectedOptionId",
                table: "BaselineAnswer");

            migrationBuilder.DropForeignKey(
                name: "FK_BaselineAnswer_BaselineQuestions_QuestionId",
                table: "BaselineAnswer");

            migrationBuilder.DropForeignKey(
                name: "FK_BaselineOption_BaselineQuestions_QuestionId",
                table: "BaselineOption");

            migrationBuilder.DropForeignKey(
                name: "FK_KnowledgeAnswer_KnowledgeCheckSubmissions_SubmissionId",
                table: "KnowledgeAnswer");

            migrationBuilder.DropForeignKey(
                name: "FK_KnowledgeAnswer_KnowledgeQuestion_QuestionId",
                table: "KnowledgeAnswer");

            migrationBuilder.DropForeignKey(
                name: "FK_KnowledgeQuestion_IndustryTasks_TaskId",
                table: "KnowledgeQuestion");

            migrationBuilder.DropForeignKey(
                name: "FK_TaskCheck_IndustryTasks_TaskId",
                table: "TaskCheck");

            migrationBuilder.DropForeignKey(
                name: "FK_TaskCheckResult_TaskAttempts_AttemptId",
                table: "TaskCheckResult");

            migrationBuilder.DropForeignKey(
                name: "FK_TaskCheckResult_TaskCheck_CheckId",
                table: "TaskCheckResult");

            migrationBuilder.DropForeignKey(
                name: "FK_TaskSkill_IndustryTasks_TaskId",
                table: "TaskSkill");

            migrationBuilder.DropForeignKey(
                name: "FK_TaskSkill_Skills_SkillId",
                table: "TaskSkill");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TaskSkill",
                table: "TaskSkill");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TaskCheckResult",
                table: "TaskCheckResult");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TaskCheck",
                table: "TaskCheck");

            migrationBuilder.DropPrimaryKey(
                name: "PK_KnowledgeQuestion",
                table: "KnowledgeQuestion");

            migrationBuilder.DropPrimaryKey(
                name: "PK_KnowledgeAnswer",
                table: "KnowledgeAnswer");

            migrationBuilder.DropPrimaryKey(
                name: "PK_BaselineOption",
                table: "BaselineOption");

            migrationBuilder.DropPrimaryKey(
                name: "PK_BaselineAnswer",
                table: "BaselineAnswer");

            migrationBuilder.RenameTable(
                name: "TaskSkill",
                newName: "TaskSkills");

            migrationBuilder.RenameTable(
                name: "TaskCheckResult",
                newName: "TaskCheckResults");

            migrationBuilder.RenameTable(
                name: "TaskCheck",
                newName: "TaskChecks");

            migrationBuilder.RenameTable(
                name: "KnowledgeQuestion",
                newName: "KnowledgeQuestions");

            migrationBuilder.RenameTable(
                name: "KnowledgeAnswer",
                newName: "KnowledgeAnswers");

            migrationBuilder.RenameTable(
                name: "BaselineOption",
                newName: "BaselineOptions");

            migrationBuilder.RenameTable(
                name: "BaselineAnswer",
                newName: "BaselineAnswers");

            migrationBuilder.RenameIndex(
                name: "IX_TaskSkill_TaskId_SkillId",
                table: "TaskSkills",
                newName: "IX_TaskSkills_TaskId_SkillId");

            migrationBuilder.RenameIndex(
                name: "IX_TaskSkill_SkillId",
                table: "TaskSkills",
                newName: "IX_TaskSkills_SkillId");

            migrationBuilder.RenameIndex(
                name: "IX_TaskCheckResult_CheckId",
                table: "TaskCheckResults",
                newName: "IX_TaskCheckResults_CheckId");

            migrationBuilder.RenameIndex(
                name: "IX_TaskCheckResult_AttemptId_CheckId",
                table: "TaskCheckResults",
                newName: "IX_TaskCheckResults_AttemptId_CheckId");

            migrationBuilder.RenameIndex(
                name: "IX_TaskCheck_TaskId",
                table: "TaskChecks",
                newName: "IX_TaskChecks_TaskId");

            migrationBuilder.RenameIndex(
                name: "IX_KnowledgeQuestion_TaskId",
                table: "KnowledgeQuestions",
                newName: "IX_KnowledgeQuestions_TaskId");

            migrationBuilder.RenameIndex(
                name: "IX_KnowledgeAnswer_SubmissionId_QuestionId",
                table: "KnowledgeAnswers",
                newName: "IX_KnowledgeAnswers_SubmissionId_QuestionId");

            migrationBuilder.RenameIndex(
                name: "IX_KnowledgeAnswer_QuestionId",
                table: "KnowledgeAnswers",
                newName: "IX_KnowledgeAnswers_QuestionId");

            migrationBuilder.RenameIndex(
                name: "IX_BaselineOption_QuestionId",
                table: "BaselineOptions",
                newName: "IX_BaselineOptions_QuestionId");

            migrationBuilder.RenameIndex(
                name: "IX_BaselineAnswer_SelectedOptionId",
                table: "BaselineAnswers",
                newName: "IX_BaselineAnswers_SelectedOptionId");

            migrationBuilder.RenameIndex(
                name: "IX_BaselineAnswer_QuestionId",
                table: "BaselineAnswers",
                newName: "IX_BaselineAnswers_QuestionId");

            migrationBuilder.RenameIndex(
                name: "IX_BaselineAnswer_AttemptId_QuestionId",
                table: "BaselineAnswers",
                newName: "IX_BaselineAnswers_AttemptId_QuestionId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_TaskSkills",
                table: "TaskSkills",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_TaskCheckResults",
                table: "TaskCheckResults",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_TaskChecks",
                table: "TaskChecks",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_KnowledgeQuestions",
                table: "KnowledgeQuestions",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_KnowledgeAnswers",
                table: "KnowledgeAnswers",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_BaselineOptions",
                table: "BaselineOptions",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_BaselineAnswers",
                table: "BaselineAnswers",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_BaselineAnswers_BaselineAttempts_AttemptId",
                table: "BaselineAnswers",
                column: "AttemptId",
                principalTable: "BaselineAttempts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_BaselineAnswers_BaselineOptions_SelectedOptionId",
                table: "BaselineAnswers",
                column: "SelectedOptionId",
                principalTable: "BaselineOptions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_BaselineAnswers_BaselineQuestions_QuestionId",
                table: "BaselineAnswers",
                column: "QuestionId",
                principalTable: "BaselineQuestions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_BaselineOptions_BaselineQuestions_QuestionId",
                table: "BaselineOptions",
                column: "QuestionId",
                principalTable: "BaselineQuestions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_KnowledgeAnswers_KnowledgeCheckSubmissions_SubmissionId",
                table: "KnowledgeAnswers",
                column: "SubmissionId",
                principalTable: "KnowledgeCheckSubmissions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_KnowledgeAnswers_KnowledgeQuestions_QuestionId",
                table: "KnowledgeAnswers",
                column: "QuestionId",
                principalTable: "KnowledgeQuestions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_KnowledgeQuestions_IndustryTasks_TaskId",
                table: "KnowledgeQuestions",
                column: "TaskId",
                principalTable: "IndustryTasks",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TaskCheckResults_TaskAttempts_AttemptId",
                table: "TaskCheckResults",
                column: "AttemptId",
                principalTable: "TaskAttempts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TaskCheckResults_TaskChecks_CheckId",
                table: "TaskCheckResults",
                column: "CheckId",
                principalTable: "TaskChecks",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_TaskChecks_IndustryTasks_TaskId",
                table: "TaskChecks",
                column: "TaskId",
                principalTable: "IndustryTasks",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TaskSkills_IndustryTasks_TaskId",
                table: "TaskSkills",
                column: "TaskId",
                principalTable: "IndustryTasks",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TaskSkills_Skills_SkillId",
                table: "TaskSkills",
                column: "SkillId",
                principalTable: "Skills",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BaselineAnswers_BaselineAttempts_AttemptId",
                table: "BaselineAnswers");

            migrationBuilder.DropForeignKey(
                name: "FK_BaselineAnswers_BaselineOptions_SelectedOptionId",
                table: "BaselineAnswers");

            migrationBuilder.DropForeignKey(
                name: "FK_BaselineAnswers_BaselineQuestions_QuestionId",
                table: "BaselineAnswers");

            migrationBuilder.DropForeignKey(
                name: "FK_BaselineOptions_BaselineQuestions_QuestionId",
                table: "BaselineOptions");

            migrationBuilder.DropForeignKey(
                name: "FK_KnowledgeAnswers_KnowledgeCheckSubmissions_SubmissionId",
                table: "KnowledgeAnswers");

            migrationBuilder.DropForeignKey(
                name: "FK_KnowledgeAnswers_KnowledgeQuestions_QuestionId",
                table: "KnowledgeAnswers");

            migrationBuilder.DropForeignKey(
                name: "FK_KnowledgeQuestions_IndustryTasks_TaskId",
                table: "KnowledgeQuestions");

            migrationBuilder.DropForeignKey(
                name: "FK_TaskCheckResults_TaskAttempts_AttemptId",
                table: "TaskCheckResults");

            migrationBuilder.DropForeignKey(
                name: "FK_TaskCheckResults_TaskChecks_CheckId",
                table: "TaskCheckResults");

            migrationBuilder.DropForeignKey(
                name: "FK_TaskChecks_IndustryTasks_TaskId",
                table: "TaskChecks");

            migrationBuilder.DropForeignKey(
                name: "FK_TaskSkills_IndustryTasks_TaskId",
                table: "TaskSkills");

            migrationBuilder.DropForeignKey(
                name: "FK_TaskSkills_Skills_SkillId",
                table: "TaskSkills");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TaskSkills",
                table: "TaskSkills");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TaskChecks",
                table: "TaskChecks");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TaskCheckResults",
                table: "TaskCheckResults");

            migrationBuilder.DropPrimaryKey(
                name: "PK_KnowledgeQuestions",
                table: "KnowledgeQuestions");

            migrationBuilder.DropPrimaryKey(
                name: "PK_KnowledgeAnswers",
                table: "KnowledgeAnswers");

            migrationBuilder.DropPrimaryKey(
                name: "PK_BaselineOptions",
                table: "BaselineOptions");

            migrationBuilder.DropPrimaryKey(
                name: "PK_BaselineAnswers",
                table: "BaselineAnswers");

            migrationBuilder.RenameTable(
                name: "TaskSkills",
                newName: "TaskSkill");

            migrationBuilder.RenameTable(
                name: "TaskChecks",
                newName: "TaskCheck");

            migrationBuilder.RenameTable(
                name: "TaskCheckResults",
                newName: "TaskCheckResult");

            migrationBuilder.RenameTable(
                name: "KnowledgeQuestions",
                newName: "KnowledgeQuestion");

            migrationBuilder.RenameTable(
                name: "KnowledgeAnswers",
                newName: "KnowledgeAnswer");

            migrationBuilder.RenameTable(
                name: "BaselineOptions",
                newName: "BaselineOption");

            migrationBuilder.RenameTable(
                name: "BaselineAnswers",
                newName: "BaselineAnswer");

            migrationBuilder.RenameIndex(
                name: "IX_TaskSkills_TaskId_SkillId",
                table: "TaskSkill",
                newName: "IX_TaskSkill_TaskId_SkillId");

            migrationBuilder.RenameIndex(
                name: "IX_TaskSkills_SkillId",
                table: "TaskSkill",
                newName: "IX_TaskSkill_SkillId");

            migrationBuilder.RenameIndex(
                name: "IX_TaskChecks_TaskId",
                table: "TaskCheck",
                newName: "IX_TaskCheck_TaskId");

            migrationBuilder.RenameIndex(
                name: "IX_TaskCheckResults_CheckId",
                table: "TaskCheckResult",
                newName: "IX_TaskCheckResult_CheckId");

            migrationBuilder.RenameIndex(
                name: "IX_TaskCheckResults_AttemptId_CheckId",
                table: "TaskCheckResult",
                newName: "IX_TaskCheckResult_AttemptId_CheckId");

            migrationBuilder.RenameIndex(
                name: "IX_KnowledgeQuestions_TaskId",
                table: "KnowledgeQuestion",
                newName: "IX_KnowledgeQuestion_TaskId");

            migrationBuilder.RenameIndex(
                name: "IX_KnowledgeAnswers_SubmissionId_QuestionId",
                table: "KnowledgeAnswer",
                newName: "IX_KnowledgeAnswer_SubmissionId_QuestionId");

            migrationBuilder.RenameIndex(
                name: "IX_KnowledgeAnswers_QuestionId",
                table: "KnowledgeAnswer",
                newName: "IX_KnowledgeAnswer_QuestionId");

            migrationBuilder.RenameIndex(
                name: "IX_BaselineOptions_QuestionId",
                table: "BaselineOption",
                newName: "IX_BaselineOption_QuestionId");

            migrationBuilder.RenameIndex(
                name: "IX_BaselineAnswers_SelectedOptionId",
                table: "BaselineAnswer",
                newName: "IX_BaselineAnswer_SelectedOptionId");

            migrationBuilder.RenameIndex(
                name: "IX_BaselineAnswers_QuestionId",
                table: "BaselineAnswer",
                newName: "IX_BaselineAnswer_QuestionId");

            migrationBuilder.RenameIndex(
                name: "IX_BaselineAnswers_AttemptId_QuestionId",
                table: "BaselineAnswer",
                newName: "IX_BaselineAnswer_AttemptId_QuestionId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_TaskSkill",
                table: "TaskSkill",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_TaskCheck",
                table: "TaskCheck",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_TaskCheckResult",
                table: "TaskCheckResult",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_KnowledgeQuestion",
                table: "KnowledgeQuestion",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_KnowledgeAnswer",
                table: "KnowledgeAnswer",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_BaselineOption",
                table: "BaselineOption",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_BaselineAnswer",
                table: "BaselineAnswer",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_BaselineAnswer_BaselineAttempts_AttemptId",
                table: "BaselineAnswer",
                column: "AttemptId",
                principalTable: "BaselineAttempts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_BaselineAnswer_BaselineOption_SelectedOptionId",
                table: "BaselineAnswer",
                column: "SelectedOptionId",
                principalTable: "BaselineOption",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_BaselineAnswer_BaselineQuestions_QuestionId",
                table: "BaselineAnswer",
                column: "QuestionId",
                principalTable: "BaselineQuestions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_BaselineOption_BaselineQuestions_QuestionId",
                table: "BaselineOption",
                column: "QuestionId",
                principalTable: "BaselineQuestions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_KnowledgeAnswer_KnowledgeCheckSubmissions_SubmissionId",
                table: "KnowledgeAnswer",
                column: "SubmissionId",
                principalTable: "KnowledgeCheckSubmissions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_KnowledgeAnswer_KnowledgeQuestion_QuestionId",
                table: "KnowledgeAnswer",
                column: "QuestionId",
                principalTable: "KnowledgeQuestion",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_KnowledgeQuestion_IndustryTasks_TaskId",
                table: "KnowledgeQuestion",
                column: "TaskId",
                principalTable: "IndustryTasks",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TaskCheck_IndustryTasks_TaskId",
                table: "TaskCheck",
                column: "TaskId",
                principalTable: "IndustryTasks",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TaskCheckResult_TaskAttempts_AttemptId",
                table: "TaskCheckResult",
                column: "AttemptId",
                principalTable: "TaskAttempts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TaskCheckResult_TaskCheck_CheckId",
                table: "TaskCheckResult",
                column: "CheckId",
                principalTable: "TaskCheck",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_TaskSkill_IndustryTasks_TaskId",
                table: "TaskSkill",
                column: "TaskId",
                principalTable: "IndustryTasks",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TaskSkill_Skills_SkillId",
                table: "TaskSkill",
                column: "SkillId",
                principalTable: "Skills",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
