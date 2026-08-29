using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace App.infra.Persistence.Migrations.ReadDb
{
    /// <inheritdoc />
    public partial class AddEmployeeAdvertisementAndMarksToReadDb : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "FinalMark",
                table: "EnrollmentsRead",
                type: "float",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "FirstExamMark",
                table: "EnrollmentsRead",
                type: "float",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "SecondExamMark",
                table: "EnrollmentsRead",
                type: "float",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "WorkMark",
                table: "EnrollmentsRead",
                type: "float",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "AdvertisementsRead",
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
                    table.PrimaryKey("PK_AdvertisementsRead", x => x.Advertisement_id);
                });

            migrationBuilder.CreateTable(
                name: "EmployeesRead",
                columns: table => new
                {
                    Employee_id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    FirstName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LastName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    User_id = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmployeesRead", x => x.Employee_id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AdvertisementsRead");

            migrationBuilder.DropTable(
                name: "EmployeesRead");

            migrationBuilder.DropColumn(
                name: "FinalMark",
                table: "EnrollmentsRead");

            migrationBuilder.DropColumn(
                name: "FirstExamMark",
                table: "EnrollmentsRead");

            migrationBuilder.DropColumn(
                name: "SecondExamMark",
                table: "EnrollmentsRead");

            migrationBuilder.DropColumn(
                name: "WorkMark",
                table: "EnrollmentsRead");
        }
    }
}
