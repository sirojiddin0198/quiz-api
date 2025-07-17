using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Quiz.CSharp.Data.Migrations
{
    /// <inheritdoc />
    public partial class MissingRelationships : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_mcq_options_question_question_id",
                table: "mcq_options");

            migrationBuilder.DropForeignKey(
                name: "fk_mcq_options_questions_mcq_question_id",
                table: "mcq_options");

            migrationBuilder.DropForeignKey(
                name: "fk_question_hints_questions_question_id",
                table: "question_hints");

            migrationBuilder.DropForeignKey(
                name: "fk_test_cases_questions_code_writing_question_id",
                table: "test_cases");

            migrationBuilder.DropForeignKey(
                name: "fk_test_cases_questions_question_id",
                table: "test_cases");

            migrationBuilder.DropPrimaryKey(
                name: "pk_test_cases",
                table: "test_cases");

            migrationBuilder.DropIndex(
                name: "ix_test_cases_code_writing_question_id",
                table: "test_cases");

            migrationBuilder.DropPrimaryKey(
                name: "pk_question_hints",
                table: "question_hints");

            migrationBuilder.DropPrimaryKey(
                name: "pk_mcq_options",
                table: "mcq_options");

            migrationBuilder.DropIndex(
                name: "ix_mcq_options_mcq_question_id",
                table: "mcq_options");

            migrationBuilder.DropColumn(
                name: "code_writing_question_id",
                table: "test_cases");

            migrationBuilder.DropColumn(
                name: "mcq_question_id",
                table: "mcq_options");

            migrationBuilder.RenameTable(
                name: "test_cases",
                newName: "test_case");

            migrationBuilder.RenameTable(
                name: "question_hints",
                newName: "question_hint");

            migrationBuilder.RenameTable(
                name: "mcq_options",
                newName: "mcq_option");

            migrationBuilder.RenameIndex(
                name: "ix_test_cases_question_id",
                table: "test_case",
                newName: "ix_test_case_question_id");

            migrationBuilder.RenameIndex(
                name: "ix_question_hints_question_id",
                table: "question_hint",
                newName: "ix_question_hint_question_id");

            migrationBuilder.RenameIndex(
                name: "ix_mcq_options_question_id",
                table: "mcq_option",
                newName: "ix_mcq_option_question_id");

            migrationBuilder.AddPrimaryKey(
                name: "pk_test_case",
                table: "test_case",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "pk_question_hint",
                table: "question_hint",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "pk_mcq_option",
                table: "mcq_option",
                column: "id");

            migrationBuilder.AddForeignKey(
                name: "fk_mcq_option_question_question_id",
                table: "mcq_option",
                column: "question_id",
                principalTable: "questions",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_question_hint_question_question_id",
                table: "question_hint",
                column: "question_id",
                principalTable: "questions",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_test_case_question_question_id",
                table: "test_case",
                column: "question_id",
                principalTable: "questions",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_mcq_option_question_question_id",
                table: "mcq_option");

            migrationBuilder.DropForeignKey(
                name: "fk_question_hint_question_question_id",
                table: "question_hint");

            migrationBuilder.DropForeignKey(
                name: "fk_test_case_question_question_id",
                table: "test_case");

            migrationBuilder.DropPrimaryKey(
                name: "pk_test_case",
                table: "test_case");

            migrationBuilder.DropPrimaryKey(
                name: "pk_question_hint",
                table: "question_hint");

            migrationBuilder.DropPrimaryKey(
                name: "pk_mcq_option",
                table: "mcq_option");

            migrationBuilder.RenameTable(
                name: "test_case",
                newName: "test_cases");

            migrationBuilder.RenameTable(
                name: "question_hint",
                newName: "question_hints");

            migrationBuilder.RenameTable(
                name: "mcq_option",
                newName: "mcq_options");

            migrationBuilder.RenameIndex(
                name: "ix_test_case_question_id",
                table: "test_cases",
                newName: "ix_test_cases_question_id");

            migrationBuilder.RenameIndex(
                name: "ix_question_hint_question_id",
                table: "question_hints",
                newName: "ix_question_hints_question_id");

            migrationBuilder.RenameIndex(
                name: "ix_mcq_option_question_id",
                table: "mcq_options",
                newName: "ix_mcq_options_question_id");

            migrationBuilder.AddColumn<int>(
                name: "code_writing_question_id",
                table: "test_cases",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "mcq_question_id",
                table: "mcq_options",
                type: "integer",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "pk_test_cases",
                table: "test_cases",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "pk_question_hints",
                table: "question_hints",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "pk_mcq_options",
                table: "mcq_options",
                column: "id");

            migrationBuilder.CreateIndex(
                name: "ix_test_cases_code_writing_question_id",
                table: "test_cases",
                column: "code_writing_question_id");

            migrationBuilder.CreateIndex(
                name: "ix_mcq_options_mcq_question_id",
                table: "mcq_options",
                column: "mcq_question_id");

            migrationBuilder.AddForeignKey(
                name: "fk_mcq_options_question_question_id",
                table: "mcq_options",
                column: "question_id",
                principalTable: "questions",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_mcq_options_questions_mcq_question_id",
                table: "mcq_options",
                column: "mcq_question_id",
                principalTable: "questions",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "fk_question_hints_questions_question_id",
                table: "question_hints",
                column: "question_id",
                principalTable: "questions",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_test_cases_questions_code_writing_question_id",
                table: "test_cases",
                column: "code_writing_question_id",
                principalTable: "questions",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "fk_test_cases_questions_question_id",
                table: "test_cases",
                column: "question_id",
                principalTable: "questions",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
