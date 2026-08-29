using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace App.infra.Persistence.Migrations.App
{
    /// <inheritdoc />
    public partial class AddEmployeeAdvertisementAndMarks : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "FinalMark",
                table: "CourseStudents",
                type: "float",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "FirstExamMark",
                table: "CourseStudents",
                type: "float",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "SecondExamMark",
                table: "CourseStudents",
                type: "float",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "WorkMark",
                table: "CourseStudents",
                type: "float",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "FinalExamWeight",
                table: "Courses",
                type: "float",
                nullable: false,
                defaultValue: 0.20000000000000001);

            migrationBuilder.AddColumn<double>(
                name: "FirstExamWeight",
                table: "Courses",
                type: "float",
                nullable: false,
                defaultValue: 0.29999999999999999);

            migrationBuilder.AddColumn<double>(
                name: "SecondExamWeight",
                table: "Courses",
                type: "float",
                nullable: false,
                defaultValue: 0.29999999999999999);

            migrationBuilder.AddColumn<double>(
                name: "WorkWeight",
                table: "Courses",
                type: "float",
                nullable: false,
                defaultValue: 0.20000000000000001);

            migrationBuilder.CreateTable(
                name: "Advertisements",
                columns: table => new
                {
                    Advertisement_id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PublishedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Advertisements", x => x.Advertisement_id);
                });

            migrationBuilder.CreateTable(
                name: "Employees",
                columns: table => new
                {
                    Employee_id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    FirstName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LastName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    User_id = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Employees", x => x.Employee_id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Advertisements");

            migrationBuilder.DropTable(
                name: "Employees");

            migrationBuilder.DropColumn(
                name: "FinalMark",
                table: "CourseStudents");

            migrationBuilder.DropColumn(
                name: "FirstExamMark",
                table: "CourseStudents");

            migrationBuilder.DropColumn(
                name: "SecondExamMark",
                table: "CourseStudents");

            migrationBuilder.DropColumn(
                name: "WorkMark",
                table: "CourseStudents");

            migrationBuilder.DropColumn(
                name: "FinalExamWeight",
                table: "Courses");

            migrationBuilder.DropColumn(
                name: "FirstExamWeight",
                table: "Courses");

            migrationBuilder.DropColumn(
                name: "SecondExamWeight",
                table: "Courses");

            migrationBuilder.DropColumn(
                name: "WorkWeight",
                table: "Courses");
        }
    }
}
