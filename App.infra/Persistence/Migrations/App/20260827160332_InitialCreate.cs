using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace App.infra.Persistence.Migrations.App
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Courses",
                columns: table => new
                {
                    Course_id = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Courses", x => x.Course_id);
                });

            migrationBuilder.CreateTable(
                name: "Students",
                columns: table => new
                {
                    Student_id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    FirstName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LastName = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Students", x => x.Student_id);
                });

            migrationBuilder.CreateTable(
                name: "CourseStudents",
                columns: table => new
                {
                    Student_id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Course_id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    EnrolledAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CourseStudents", x => new { x.Student_id, x.Course_id });
                    table.ForeignKey(
                        name: "FK_CourseStudents_Courses_Course_id",
                        column: x => x.Course_id,
                        principalTable: "Courses",
                        principalColumn: "Course_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CourseStudents_Students_Student_id",
                        column: x => x.Student_id,
                        principalTable: "Students",
                        principalColumn: "Student_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CourseStudents_Course_id",
                table: "CourseStudents",
                column: "Course_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CourseStudents");

            migrationBuilder.DropTable(
                name: "Courses");

            migrationBuilder.DropTable(
                name: "Students");
        }
    }
}
