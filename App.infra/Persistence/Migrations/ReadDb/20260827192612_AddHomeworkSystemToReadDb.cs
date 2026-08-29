using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace App.infra.Persistence.Migrations.ReadDb
{
    /// <inheritdoc />
    public partial class AddHomeworkSystemToReadDb : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "HomeworkQuestionMarksRead",
                columns: table => new
                {
                    QuestionMark_id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Submission_id = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    QuestionNumber = table.Column<int>(type: "int", nullable: false),
                    QuestionDescription = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MaxMarks = table.Column<double>(type: "float", nullable: false),
                    ObtainedMarks = table.Column<double>(type: "float", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HomeworkQuestionMarksRead", x => x.QuestionMark_id);
                });

            migrationBuilder.CreateTable(
                name: "HomeworkSolutionsRead",
                columns: table => new
                {
                    Solution_id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Homework_id = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SolutionFileName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SolutionFileUrl = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UploadedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HomeworkSolutionsRead", x => x.Solution_id);
                });

            migrationBuilder.CreateTable(
                name: "HomeworksRead",
                columns: table => new
                {
                    Homework_id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Course_id = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    QuestionFileName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    QuestionFileUrl = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DueDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TotalMarks = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HomeworksRead", x => x.Homework_id);
                });

            migrationBuilder.CreateTable(
                name: "HomeworkSubmissionsRead",
                columns: table => new
                {
                    Submission_id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Homework_id = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Student_id = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SolutionFileName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SolutionFileUrl = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SubmittedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Mark = table.Column<double>(type: "float", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HomeworkSubmissionsRead", x => x.Submission_id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "HomeworkQuestionMarksRead");

            migrationBuilder.DropTable(
                name: "HomeworkSolutionsRead");

            migrationBuilder.DropTable(
                name: "HomeworksRead");

            migrationBuilder.DropTable(
                name: "HomeworkSubmissionsRead");
        }
    }
}
