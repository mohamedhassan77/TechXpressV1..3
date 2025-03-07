using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TechXpress_infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class e : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "EmailNotifications",
                table: "UserProfiles",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "LanguagePreference",
                table: "UserProfiles",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "MarketingEmails",
                table: "UserProfiles",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "PreferredCurrency",
                table: "UserProfiles",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "ProfileVisibility",
                table: "UserProfiles",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "ShowBirthDate",
                table: "UserProfiles",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "SmsNotificationsEnabled",
                table: "UserProfiles",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "Theme",
                table: "UserProfiles",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "TwoFactorEnabled",
                table: "UserProfiles",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EmailNotifications",
                table: "UserProfiles");

            migrationBuilder.DropColumn(
                name: "LanguagePreference",
                table: "UserProfiles");

            migrationBuilder.DropColumn(
                name: "MarketingEmails",
                table: "UserProfiles");

            migrationBuilder.DropColumn(
                name: "PreferredCurrency",
                table: "UserProfiles");

            migrationBuilder.DropColumn(
                name: "ProfileVisibility",
                table: "UserProfiles");

            migrationBuilder.DropColumn(
                name: "ShowBirthDate",
                table: "UserProfiles");

            migrationBuilder.DropColumn(
                name: "SmsNotificationsEnabled",
                table: "UserProfiles");

            migrationBuilder.DropColumn(
                name: "Theme",
                table: "UserProfiles");

            migrationBuilder.DropColumn(
                name: "TwoFactorEnabled",
                table: "UserProfiles");
        }
    }
}
