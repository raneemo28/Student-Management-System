using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace App.infra.Persistence.Migrations.ReadDb
{
    /// <inheritdoc />
    public partial class InitialReadDb : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CoursesRead",
                columns: table => new
                {
                    Course_id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Course_name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CoursesRead", x => x.Course_id);
                });

            migrationBuilder.CreateTable(
                name: "EnrollmentsRead",
                columns: table => new
                {
                    Student_id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Course_id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    EnrolledAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EnrollmentsRead", x => new { x.Student_id, x.Course_id });
                });

            migrationBuilder.CreateTable(
                name: "StudentsRead",
                columns: table => new
                {
                    Student_id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    FirstName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LastName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    User_id = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StudentsRead", x => x.Student_id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CoursesRead");

            migrationBuilder.DropTable(
                name: "EnrollmentsRead");

            migrationBuilder.DropTable(
                name: "StudentsRead");
        }
    }
}
