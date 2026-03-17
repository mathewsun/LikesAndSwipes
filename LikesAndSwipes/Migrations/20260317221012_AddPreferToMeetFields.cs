using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LikesAndSwipes.Migrations
{
    /// <inheritdoc />
    public partial class AddPreferToMeetFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "FriendshipMen",
                table: "AspNetUsers",
                type: "boolean",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "FriendshipWomen",
                table: "AspNetUsers",
                type: "boolean",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "RomanticMen",
                table: "AspNetUsers",
                type: "boolean",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "RomanticWomen",
                table: "AspNetUsers",
                type: "boolean",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FriendshipMen",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "FriendshipWomen",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "RomanticMen",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "RomanticWomen",
                table: "AspNetUsers");
        }
    }
}
