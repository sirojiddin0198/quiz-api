using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Quiz.CSharp.Data.Migrations
{
    /// <inheritdoc />
    public partial class UpdateMCQOptionCompoundKey : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "pk_mcq_option",
                table: "mcq_option");

            migrationBuilder.AddPrimaryKey(
                name: "pk_mcq_option",
                table: "mcq_option",
                columns: new[] { "id", "question_id" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "pk_mcq_option",
                table: "mcq_option");

            migrationBuilder.AddPrimaryKey(
                name: "pk_mcq_option",
                table: "mcq_option",
                column: "id");
        }
    }
}
