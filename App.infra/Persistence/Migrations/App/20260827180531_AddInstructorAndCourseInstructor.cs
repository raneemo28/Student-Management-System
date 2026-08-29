using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace App.infra.Persistence.Migrations.App
{
    /// <inheritdoc />
    public partial class AddInstructorAndCourseInstructor : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Instructor_id",
                table: "Courses",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Instructors",
                columns: table => new
                {
                    Instructor_id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    FirstName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LastName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    User_id = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Instructors", x => x.Instructor_id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Courses_Instructor_id",
                table: "Courses",
                column: "Instructor_id");

            migrationBuilder.AddForeignKey(
                name: "FK_Courses_Instructors_Instructor_id",
                table: "Courses",
                column: "Instructor_id",
                principalTable: "Instructors",
                principalColumn: "Instructor_id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Courses_Instructors_Instructor_id",
                table: "Courses");

            migrationBuilder.DropTable(
                name: "Instructors");

            migrationBuilder.DropIndex(
                name: "IX_Courses_Instructor_id",
                table: "Courses");

            migrationBuilder.DropColumn(
                name: "Instructor_id",
                table: "Courses");
        }
    }
}
