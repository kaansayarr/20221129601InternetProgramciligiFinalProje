using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Uyg.API.Migrations
{
    /// <inheritdoc />
    public partial class AddCourseTableAndRelations : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Lessons_AspNetUsers_UserId",
                table: "Lessons");

            migrationBuilder.DropIndex(
                name: "IX_Lessons_UserId",
                table: "Lessons");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Lessons");

            // Create Courses table first
            migrationBuilder.CreateTable(
                name: "Courses",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PhotoUrl = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Price = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    Created = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Updated = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Courses", x => x.Id);
                });

            // Insert a default course
            migrationBuilder.InsertData(
                table: "Courses",
                columns: new[] { "Name", "Description", "PhotoUrl", "Price", "IsActive", "Created", "Updated" },
                values: new object[] { "Default Course", "Default Course Description", "default.jpg", 0, true, DateTime.Now, DateTime.Now }
            );

            // Add CourseId column to Lessons
            migrationBuilder.AddColumn<int>(
                name: "CourseId",
                table: "Lessons",
                type: "int",
                nullable: false,
                defaultValue: 1); // Set default value to 1 (the ID of our default course)

            migrationBuilder.CreateIndex(
                name: "IX_Lessons_CourseId",
                table: "Lessons",
                column: "CourseId");

            migrationBuilder.AddForeignKey(
                name: "FK_Lessons_Courses_CourseId",
                table: "Lessons",
                column: "CourseId",
                principalTable: "Courses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Lessons_Courses_CourseId",
                table: "Lessons");

            migrationBuilder.DropTable(
                name: "Courses");

            migrationBuilder.DropIndex(
                name: "IX_Lessons_CourseId",
                table: "Lessons");

            migrationBuilder.DropColumn(
                name: "CourseId",
                table: "Lessons");

            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "Lessons",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_Lessons_UserId",
                table: "Lessons",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Lessons_AspNetUsers_UserId",
                table: "Lessons",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
