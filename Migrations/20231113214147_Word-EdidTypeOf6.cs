using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AppQuizlet.Migrations
{
    /// <inheritdoc />
    public partial class WordEdidTypeOf6 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_words_libraries_LibraryEnglishId1",
                table: "words");

            migrationBuilder.DropIndex(
                name: "IX_words_LibraryEnglishId1",
                table: "words");

            migrationBuilder.DropColumn(
                name: "LibraryEnglishId1",
                table: "words");

            migrationBuilder.AlterColumn<int>(
                name: "LibraryEnglishId",
                table: "words",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.CreateIndex(
                name: "IX_words_LibraryEnglishId",
                table: "words",
                column: "LibraryEnglishId");

            migrationBuilder.AddForeignKey(
                name: "FK_words_libraries_LibraryEnglishId",
                table: "words",
                column: "LibraryEnglishId",
                principalTable: "libraries",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_words_libraries_LibraryEnglishId",
                table: "words");

            migrationBuilder.DropIndex(
                name: "IX_words_LibraryEnglishId",
                table: "words");

            migrationBuilder.AlterColumn<string>(
                name: "LibraryEnglishId",
                table: "words",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<int>(
                name: "LibraryEnglishId1",
                table: "words",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_words_LibraryEnglishId1",
                table: "words",
                column: "LibraryEnglishId1");

            migrationBuilder.AddForeignKey(
                name: "FK_words_libraries_LibraryEnglishId1",
                table: "words",
                column: "LibraryEnglishId1",
                principalTable: "libraries",
                principalColumn: "Id");
        }
    }
}
