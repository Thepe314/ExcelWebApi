using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ExcelUploadApi.Migrations
{
    /// <inheritdoc />
    public partial class AddSemesterColumns : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Semester",
                table: "Subjects",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "SemesterYear",
                table: "Subjects",
                type: "int",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Semester",
                table: "Subjects");

            migrationBuilder.DropColumn(
                name: "SemesterYear",
                table: "Subjects");
        }
    }
}
