using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProductsApi.Migrations
{
    /// <inheritdoc />
    public partial class AnimalBinary : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ApiLink",
                table: "Photos",
                newName: "ImageExtension");

            migrationBuilder.AddColumn<byte[]>(
                name: "ImageData",
                table: "Photos",
                type: "varbinary(max)",
                nullable: false,
                defaultValue: new byte[0]);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ImageData",
                table: "Photos");

            migrationBuilder.RenameColumn(
                name: "ImageExtension",
                table: "Photos",
                newName: "ApiLink");
        }
    }
}
