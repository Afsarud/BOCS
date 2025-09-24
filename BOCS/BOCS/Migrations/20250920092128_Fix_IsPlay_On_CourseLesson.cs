using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BOCS.Migrations
{
    /// <inheritdoc />
    public partial class Fix_IsPlay_On_CourseLesson : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
IF COL_LENGTH(N'dbo.CourseLessons', N'IsPlay') IS NULL
BEGIN
    ALTER TABLE [dbo].[CourseLessons]
        ADD [IsPlay] bit NOT NULL 
            CONSTRAINT [DF_CourseLessons_IsPlay] DEFAULT(0);
END
");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
IF COL_LENGTH(N'dbo.CourseLessons', N'IsPlay') IS NOT NULL
BEGIN
    ALTER TABLE [dbo].[CourseLessons] DROP CONSTRAINT [DF_CourseLessons_IsPlay];
    ALTER TABLE [dbo].[CourseLessons] DROP COLUMN [IsPlay];
END
");
        }
    }
}
