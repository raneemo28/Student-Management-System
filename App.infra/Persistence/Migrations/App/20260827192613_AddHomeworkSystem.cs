using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace App.infra.Persistence.Migrations.App
{
    /// <inheritdoc />
    public partial class AddHomeworkSystem : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<double>(
                name: "WorkWeight",
                table: "Courses",
                type: "float",
                nullable: false,
                oldClrType: typeof(double),
                oldType: "float",
                oldDefaultValue: 0.20000000000000001);

            migrationBuilder.AlterColumn<double>(
                name: "SecondExamWeight",
                table: "Courses",
                type: "float",
                nullable: false,
                oldClrType: typeof(double),
                oldType: "float",
                oldDefaultValue: 0.29999999999999999);

            migrationBuilder.AlterColumn<double>(
                name: "FirstExamWeight",
                table: "Courses",
                type: "float",
                nullable: false,
                oldClrType: typeof(double),
                oldType: "float",
                oldDefaultValue: 0.29999999999999999);

            migrationBuilder.AlterColumn<double>(
                name: "FinalExamWeight",
                table: "Courses",
                type: "float",
                nullable: false,
                oldClrType: typeof(double),
                oldType: "float",
                oldDefaultValue: 0.20000000000000001);

            migrationBuilder.CreateTable(
                name: "Homeworks",
                columns: table => new
                {
                    Homework_id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Course_id = table.Column<string>(type: "nvarchar(450)", nullable: false),
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
                    table.PrimaryKey("PK_Homeworks", x => x.Homework_id);
                    table.ForeignKey(
                        name: "FK_Homeworks_Courses_Course_id",
                        column: x => x.Course_id,
                        principalTable: "Courses",
                        principalColumn: "Course_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "HomeworkSolutions",
                columns: table => new
                {
                    Solution_id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Homework_id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    SolutionFileName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SolutionFileUrl = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UploadedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HomeworkSolutions", x => x.Solution_id);
                    table.ForeignKey(
                        name: "FK_HomeworkSolutions_Homeworks_Homework_id",
                        column: x => x.Homework_id,
                        principalTable: "Homeworks",
                        principalColumn: "Homework_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "HomeworkSubmissions",
                columns: table => new
                {
                    Submission_id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Homework_id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Student_id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    SolutionFileName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SolutionFileUrl = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SubmittedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Mark = table.Column<double>(type: "float", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HomeworkSubmissions", x => x.Submission_id);
                    table.ForeignKey(
                        name: "FK_HomeworkSubmissions_Homeworks_Homework_id",
                        column: x => x.Homework_id,
                        principalTable: "Homeworks",
                        principalColumn: "Homework_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_HomeworkSubmissions_Students_Student_id",
                        column: x => x.Student_id,
                        principalTable: "Students",
                        principalColumn: "Student_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "HomeworkQuestionMarks",
                columns: table => new
                {
                    QuestionMark_id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Submission_id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    QuestionNumber = table.Column<int>(type: "int", nullable: false),
                    QuestionDescription = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MaxMarks = table.Column<double>(type: "float", nullable: false),
                    ObtainedMarks = table.Column<double>(type: "float", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HomeworkQuestionMarks", x => x.QuestionMark_id);
                    table.ForeignKey(
                        name: "FK_HomeworkQuestionMarks_HomeworkSubmissions_Submission_id",
                        column: x => x.Submission_id,
                        principalTable: "HomeworkSubmissions",
                        principalColumn: "Submission_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_HomeworkQuestionMarks_Submission_id",
                table: "HomeworkQuestionMarks",
                column: "Submission_id");

            migrationBuilder.CreateIndex(
                name: "IX_Homeworks_Course_id",
                table: "Homeworks",
                column: "Course_id");

            migrationBuilder.CreateIndex(
                name: "IX_HomeworkSolutions_Homework_id",
                table: "HomeworkSolutions",
                column: "Homework_id");

            migrationBuilder.CreateIndex(
                name: "IX_HomeworkSubmissions_Homework_id",
                table: "HomeworkSubmissions",
                column: "Homework_id");

            migrationBuilder.CreateIndex(
                name: "IX_HomeworkSubmissions_Student_id",
                table: "HomeworkSubmissions",
                column: "Student_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "HomeworkQuestionMarks");

            migrationBuilder.DropTable(
                name: "HomeworkSolutions");

            migrationBuilder.DropTable(
                name: "HomeworkSubmissions");

            migrationBuilder.DropTable(
                name: "Homeworks");

            migrationBuilder.AlterColumn<double>(
                name: "WorkWeight",
                table: "Courses",
                type: "float",
                nullable: false,
                defaultValue: 0.20000000000000001,
                oldClrType: typeof(double),
                oldType: "float");

            migrationBuilder.AlterColumn<double>(
                name: "SecondExamWeight",
                table: "Courses",
                type: "float",
                nullable: false,
                defaultValue: 0.29999999999999999,
                oldClrType: typeof(double),
                oldType: "float");

            migrationBuilder.AlterColumn<double>(
                name: "FirstExamWeight",
                table: "Courses",
                type: "float",
                nullable: false,
                defaultValue: 0.29999999999999999,
                oldClrType: typeof(double),
                oldType: "float");

            migrationBuilder.AlterColumn<double>(
                name: "FinalExamWeight",
                table: "Courses",
                type: "float",
                nullable: false,
                defaultValue: 0.20000000000000001,
                oldClrType: typeof(double),
                oldType: "float");
        }
    }
}
