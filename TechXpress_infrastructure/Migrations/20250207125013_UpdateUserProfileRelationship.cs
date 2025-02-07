using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TechXpress_infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateUserProfileRelationship : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_UserProfiles_Id",
                table: "AspNetUsers");

            migrationBuilder.AddForeignKey(
                name: "FK_UserProfiles_AspNetUsers_Id",
                table: "UserProfiles",
                column: "Id",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserProfiles_AspNetUsers_Id",
                table: "UserProfiles");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_UserProfiles_Id",
                table: "AspNetUsers",
                column: "Id",
                principalTable: "UserProfiles",
                principalColumn: "Id");
        }
    }
}
