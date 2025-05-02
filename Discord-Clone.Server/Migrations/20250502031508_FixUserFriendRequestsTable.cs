using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Discord_Clone.Server.Migrations
{
    /// <inheritdoc />
    public partial class FixUserFriendRequestsTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserFriendRequests_AspNetUsers_SenderId1",
                table: "UserFriendRequests");

            migrationBuilder.DropIndex(
                name: "IX_UserFriendRequests_SenderId1",
                table: "UserFriendRequests");

            migrationBuilder.DropColumn(
                name: "SenderId1",
                table: "UserFriendRequests");

            migrationBuilder.CreateIndex(
                name: "IX_UserFriendRequests_ReceiverId",
                table: "UserFriendRequests",
                column: "ReceiverId");

            migrationBuilder.AddForeignKey(
                name: "FK_UserFriendRequests_AspNetUsers_ReceiverId",
                table: "UserFriendRequests",
                column: "ReceiverId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserFriendRequests_AspNetUsers_ReceiverId",
                table: "UserFriendRequests");

            migrationBuilder.DropIndex(
                name: "IX_UserFriendRequests_ReceiverId",
                table: "UserFriendRequests");

            migrationBuilder.AddColumn<string>(
                name: "SenderId1",
                table: "UserFriendRequests",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_UserFriendRequests_SenderId1",
                table: "UserFriendRequests",
                column: "SenderId1");

            migrationBuilder.AddForeignKey(
                name: "FK_UserFriendRequests_AspNetUsers_SenderId1",
                table: "UserFriendRequests",
                column: "SenderId1",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
