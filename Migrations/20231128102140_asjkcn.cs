using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AppQuizlet.Migrations
{
    /// <inheritdoc />
    public partial class asjkcn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "imageUrl",
                table: "words",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "imageUrl",
                table: "words");
        }
    }
}
