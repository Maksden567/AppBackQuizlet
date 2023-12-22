using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AppQuizlet.Migrations
{
    /// <inheritdoc />
    public partial class asjkcas : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "musicUrl",
                table: "words",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "musicUrl",
                table: "words");
        }
    }
}
