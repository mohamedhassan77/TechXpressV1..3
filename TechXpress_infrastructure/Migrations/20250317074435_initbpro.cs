using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TechXpress_infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class initbpro : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ProfileImage",
                table: "UserProfiles");

            migrationBuilder.AddColumn<byte[]>(
                name: "ProfileImageData",
                table: "UserProfiles",
                type: "varbinary(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ProfileImageUrl",
                table: "UserProfiles",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ProfileImageData",
                table: "UserProfiles");

            migrationBuilder.DropColumn(
                name: "ProfileImageUrl",
                table: "UserProfiles");

            migrationBuilder.AddColumn<string>(
                name: "ProfileImage",
                table: "UserProfiles",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
