using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QuizService.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateQuizRelationship : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AnswerOptions_Questions_QuestionId1",
                table: "AnswerOptions");

            migrationBuilder.DropIndex(
                name: "IX_AnswerOptions_QuestionId1",
                table: "AnswerOptions");

            migrationBuilder.DropColumn(
                name: "PassScore",
                table: "Quizzes");

            migrationBuilder.DropColumn(
                name: "QuestionId1",
                table: "AnswerOptions");

            migrationBuilder.RenameColumn(
                name: "QuizId",
                table: "Quizzes",
                newName: "Id");

            migrationBuilder.AddColumn<double>(
                name: "PassPercentage",
                table: "Quizzes",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.CreateTable(
                name: "QuizAttempts",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    QuizId = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    StartedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    FinishedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Score = table.Column<int>(type: "integer", nullable: false),
                    TotalScore = table.Column<double>(type: "double precision", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QuizAttempts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_QuizAttempts_Quizzes_QuizId",
                        column: x => x.QuizId,
                        principalTable: "Quizzes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserAnswers",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    AttemptId = table.Column<Guid>(type: "uuid", nullable: false),
                    QuestionId = table.Column<Guid>(type: "uuid", nullable: false),
                    TextAnswer = table.Column<string>(type: "text", nullable: true),
                    SelectedOptionId = table.Column<List<Guid>>(type: "uuid[]", nullable: true),
                    IsCorrect = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserAnswers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserAnswers_QuizAttempts_AttemptId",
                        column: x => x.AttemptId,
                        principalTable: "QuizAttempts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_QuizAttempts_QuizId",
                table: "QuizAttempts",
                column: "QuizId");

            migrationBuilder.CreateIndex(
                name: "IX_UserAnswers_AttemptId",
                table: "UserAnswers",
                column: "AttemptId");

            migrationBuilder.CreateIndex(
                name: "IX_UserAnswers_QuestionId",
                table: "UserAnswers",
                column: "QuestionId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserAnswers");

            migrationBuilder.DropTable(
                name: "QuizAttempts");

            migrationBuilder.DropColumn(
                name: "PassPercentage",
                table: "Quizzes");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "Quizzes",
                newName: "QuizId");

            migrationBuilder.AddColumn<int>(
                name: "PassScore",
                table: "Quizzes",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<Guid>(
                name: "QuestionId1",
                table: "AnswerOptions",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_AnswerOptions_QuestionId1",
                table: "AnswerOptions",
                column: "QuestionId1");

            migrationBuilder.AddForeignKey(
                name: "FK_AnswerOptions_Questions_QuestionId1",
                table: "AnswerOptions",
                column: "QuestionId1",
                principalTable: "Questions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
