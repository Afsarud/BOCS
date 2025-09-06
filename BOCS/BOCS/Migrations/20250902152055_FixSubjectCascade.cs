using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BOCS.Migrations
{
    /// <inheritdoc />
    public partial class FixSubjectCascade : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "SubjectId",
                table: "CourseLessons",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "CourseSubjects",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CourseId = table.Column<int>(type: "int", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    SortOrder = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    IsPublished = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CourseSubjects", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CourseSubjects_Courses_CourseId",
                        column: x => x.CourseId,
                        principalTable: "Courses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CourseLessons_SubjectId",
                table: "CourseLessons",
                column: "SubjectId");

            migrationBuilder.CreateIndex(
                name: "IX_CourseSubjects_CourseId_SortOrder",
                table: "CourseSubjects",
                columns: new[] { "CourseId", "SortOrder" });

            migrationBuilder.AddForeignKey(
                name: "FK_CourseLessons_CourseSubjects_SubjectId",
                table: "CourseLessons",
                column: "SubjectId",
                principalTable: "CourseSubjects",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CourseLessons_CourseSubjects_SubjectId",
                table: "CourseLessons");

            migrationBuilder.DropTable(
                name: "CourseSubjects");

            migrationBuilder.DropIndex(
                name: "IX_CourseLessons_SubjectId",
                table: "CourseLessons");

            migrationBuilder.DropColumn(
                name: "SubjectId",
                table: "CourseLessons");
        }
    }
}
