using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace App.infra.Persistence.Migrations.ReadDb
{
    /// <inheritdoc />
    public partial class AddInstructorToReadDb : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Instructor_id",
                table: "CoursesRead",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "InstructorsRead",
                columns: table => new
                {
                    Instructor_id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    FirstName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LastName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    User_id = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InstructorsRead", x => x.Instructor_id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "InstructorsRead");

            migrationBuilder.DropColumn(
                name: "Instructor_id",
                table: "CoursesRead");
        }
    }
}
