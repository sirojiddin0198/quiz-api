using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Quiz.CSharp.Data.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "collections",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    code = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    title = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    description = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false),
                    icon = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    sort_order = table.Column<int>(type: "integer", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    is_active = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_collections", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "questions",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    collection_id = table.Column<int>(type: "integer", nullable: false),
                    subcategory = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    difficulty = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    prompt = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: false),
                    estimated_time_minutes = table.Column<int>(type: "integer", nullable: false),
                    question_type = table.Column<string>(type: "character varying(21)", maxLength: 21, nullable: false),
                    solution = table.Column<string>(type: "text", nullable: true),
                    examples = table.Column<string[]>(type: "text[]", nullable: true),
                    rubric = table.Column<string[]>(type: "text[]", nullable: true),
                    error_spotting_question_correct_answer = table.Column<string>(type: "text", nullable: true),
                    correct_answer = table.Column<string>(type: "text", nullable: true),
                    fill_hints = table.Column<string[]>(type: "text[]", nullable: true),
                    expected_output = table.Column<string>(type: "text", nullable: true),
                    true_false_question_correct_answer = table.Column<bool>(type: "boolean", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    is_active = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_questions", x => x.id);
                    table.ForeignKey(
                        name: "fk_questions_collections_collection_id",
                        column: x => x.collection_id,
                        principalTable: "collections",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "user_progress",
                columns: table => new
                {
                    user_id = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    collection_id = table.Column<int>(type: "integer", nullable: false),
                    total_questions = table.Column<int>(type: "integer", nullable: false),
                    answered_questions = table.Column<int>(type: "integer", nullable: false),
                    correct_answers = table.Column<int>(type: "integer", nullable: false),
                    success_rate = table.Column<decimal>(type: "numeric(5,2)", precision: 5, scale: 2, nullable: false),
                    last_answered_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    is_active = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_user_progress", x => new { x.user_id, x.collection_id });
                    table.ForeignKey(
                        name: "fk_user_progress_collection_collection_id",
                        column: x => x.collection_id,
                        principalTable: "collections",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "mcq_options",
                columns: table => new
                {
                    id = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    question_id = table.Column<int>(type: "integer", nullable: false),
                    option = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false),
                    is_correct = table.Column<bool>(type: "boolean", nullable: false),
                    mcq_question_id = table.Column<int>(type: "integer", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    is_active = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_mcq_options", x => x.id);
                    table.ForeignKey(
                        name: "fk_mcq_options_question_question_id",
                        column: x => x.question_id,
                        principalTable: "questions",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_mcq_options_questions_mcq_question_id",
                        column: x => x.mcq_question_id,
                        principalTable: "questions",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "question_hints",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    question_id = table.Column<int>(type: "integer", nullable: false),
                    hint = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false),
                    order_index = table.Column<int>(type: "integer", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    is_active = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_question_hints", x => x.id);
                    table.ForeignKey(
                        name: "fk_question_hints_questions_question_id",
                        column: x => x.question_id,
                        principalTable: "questions",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "test_cases",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    question_id = table.Column<int>(type: "integer", nullable: false),
                    input = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: false),
                    expected_output = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: false),
                    code_writing_question_id = table.Column<int>(type: "integer", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    is_active = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_test_cases", x => x.id);
                    table.ForeignKey(
                        name: "fk_test_cases_questions_code_writing_question_id",
                        column: x => x.code_writing_question_id,
                        principalTable: "questions",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "fk_test_cases_questions_question_id",
                        column: x => x.question_id,
                        principalTable: "questions",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "user_answers",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    user_id = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    question_id = table.Column<int>(type: "integer", nullable: false),
                    answer = table.Column<string>(type: "character varying(5000)", maxLength: 5000, nullable: false),
                    is_correct = table.Column<bool>(type: "boolean", nullable: false),
                    time_spent_seconds = table.Column<int>(type: "integer", nullable: false),
                    submitted_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    attempt_number = table.Column<int>(type: "integer", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    is_active = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_user_answers", x => x.id);
                    table.ForeignKey(
                        name: "fk_user_answers_questions_question_id",
                        column: x => x.question_id,
                        principalTable: "questions",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_collections_code",
                table: "collections",
                column: "code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_collections_is_active",
                table: "collections",
                column: "is_active");

            migrationBuilder.CreateIndex(
                name: "ix_collections_sort_order",
                table: "collections",
                column: "sort_order");

            migrationBuilder.CreateIndex(
                name: "ix_mcq_options_mcq_question_id",
                table: "mcq_options",
                column: "mcq_question_id");

            migrationBuilder.CreateIndex(
                name: "ix_mcq_options_question_id",
                table: "mcq_options",
                column: "question_id");

            migrationBuilder.CreateIndex(
                name: "ix_question_hints_question_id",
                table: "question_hints",
                column: "question_id");

            migrationBuilder.CreateIndex(
                name: "IX_questions_collection_id",
                table: "questions",
                column: "collection_id");

            migrationBuilder.CreateIndex(
                name: "ix_test_cases_code_writing_question_id",
                table: "test_cases",
                column: "code_writing_question_id");

            migrationBuilder.CreateIndex(
                name: "ix_test_cases_question_id",
                table: "test_cases",
                column: "question_id");

            migrationBuilder.CreateIndex(
                name: "ix_user_answers_question_id",
                table: "user_answers",
                column: "question_id");

            migrationBuilder.CreateIndex(
                name: "ix_user_answers_user_id",
                table: "user_answers",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "ix_user_answers_user_id_question_id",
                table: "user_answers",
                columns: new[] { "user_id", "question_id" });

            migrationBuilder.CreateIndex(
                name: "ix_user_progress_collection_id",
                table: "user_progress",
                column: "collection_id");

            migrationBuilder.CreateIndex(
                name: "ix_user_progress_user_id",
                table: "user_progress",
                column: "user_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "mcq_options");

            migrationBuilder.DropTable(
                name: "question_hints");

            migrationBuilder.DropTable(
                name: "test_cases");

            migrationBuilder.DropTable(
                name: "user_answers");

            migrationBuilder.DropTable(
                name: "user_progress");

            migrationBuilder.DropTable(
                name: "questions");

            migrationBuilder.DropTable(
                name: "collections");
        }
    }
}
