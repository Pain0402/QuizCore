using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QuizCore.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Phase5_AttemptFlow : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AttemptDetails_Answers_SelectedAnswerId",
                table: "AttemptDetails");

            migrationBuilder.DropColumn(
                name: "TotalScore",
                table: "ExamAttempts");

            migrationBuilder.DropColumn(
                name: "TextAnswer",
                table: "AttemptDetails");

            migrationBuilder.RenameColumn(
                name: "Type",
                table: "Questions",
                newName: "QuestionType");

            migrationBuilder.RenameColumn(
                name: "SubmitTime",
                table: "ExamAttempts",
                newName: "SubmittedAt");

            migrationBuilder.RenameColumn(
                name: "StartTime",
                table: "ExamAttempts",
                newName: "StartedAt");

            migrationBuilder.RenameColumn(
                name: "SelectedAnswerId",
                table: "AttemptDetails",
                newName: "AnswerId");

            migrationBuilder.RenameIndex(
                name: "IX_AttemptDetails_SelectedAnswerId",
                table: "AttemptDetails",
                newName: "IX_AttemptDetails_AnswerId");

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "Exams",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "PassMark",
                table: "Exams",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "TotalMark",
                table: "Exams",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<int>(
                name: "Order",
                table: "ExamQuestions",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<decimal>(
                name: "Score",
                table: "ExamAttempts",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AlterColumn<bool>(
                name: "IsCorrect",
                table: "AttemptDetails",
                type: "boolean",
                nullable: false,
                defaultValue: false,
                oldClrType: typeof(bool),
                oldType: "boolean",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_AttemptDetails_Answers_AnswerId",
                table: "AttemptDetails",
                column: "AnswerId",
                principalTable: "Answers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AttemptDetails_Answers_AnswerId",
                table: "AttemptDetails");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "Exams");

            migrationBuilder.DropColumn(
                name: "PassMark",
                table: "Exams");

            migrationBuilder.DropColumn(
                name: "TotalMark",
                table: "Exams");

            migrationBuilder.DropColumn(
                name: "Order",
                table: "ExamQuestions");

            migrationBuilder.DropColumn(
                name: "Score",
                table: "ExamAttempts");

            migrationBuilder.RenameColumn(
                name: "QuestionType",
                table: "Questions",
                newName: "Type");

            migrationBuilder.RenameColumn(
                name: "SubmittedAt",
                table: "ExamAttempts",
                newName: "SubmitTime");

            migrationBuilder.RenameColumn(
                name: "StartedAt",
                table: "ExamAttempts",
                newName: "StartTime");

            migrationBuilder.RenameColumn(
                name: "AnswerId",
                table: "AttemptDetails",
                newName: "SelectedAnswerId");

            migrationBuilder.RenameIndex(
                name: "IX_AttemptDetails_AnswerId",
                table: "AttemptDetails",
                newName: "IX_AttemptDetails_SelectedAnswerId");

            migrationBuilder.AddColumn<float>(
                name: "TotalScore",
                table: "ExamAttempts",
                type: "real",
                nullable: true);

            migrationBuilder.AlterColumn<bool>(
                name: "IsCorrect",
                table: "AttemptDetails",
                type: "boolean",
                nullable: true,
                oldClrType: typeof(bool),
                oldType: "boolean");

            migrationBuilder.AddColumn<string>(
                name: "TextAnswer",
                table: "AttemptDetails",
                type: "text",
                nullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_AttemptDetails_Answers_SelectedAnswerId",
                table: "AttemptDetails",
                column: "SelectedAnswerId",
                principalTable: "Answers",
                principalColumn: "Id");
        }
    }
}
