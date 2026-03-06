using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ExcelUploadApi.Migrations
{
    /// <inheritdoc />
    public partial class AddCgpaColumn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "CGPA",
                table: "Subjects",
                type: "decimal(18,2)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CGPA",
                table: "Subjects");
        }
    }
}
