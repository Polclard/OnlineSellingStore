using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OnlineSellingStore.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class AddFriendBookInDb1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FriendBooks_FriendPublishers_FriendPublisherId",
                table: "FriendBooks");

            migrationBuilder.RenameColumn(
                name: "FriendPublisherId",
                table: "FriendBooks",
                newName: "PublisherId");

            migrationBuilder.RenameIndex(
                name: "IX_FriendBooks_FriendPublisherId",
                table: "FriendBooks",
                newName: "IX_FriendBooks_PublisherId");

            migrationBuilder.RenameColumn(
                name: "FriendAuthorName",
                table: "FriendAuthors",
                newName: "Bio");

            migrationBuilder.RenameColumn(
                name: "FriendAuthorBio",
                table: "FriendAuthors",
                newName: "AuthorName");

            migrationBuilder.AddForeignKey(
                name: "FK_FriendBooks_FriendPublishers_PublisherId",
                table: "FriendBooks",
                column: "PublisherId",
                principalTable: "FriendPublishers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FriendBooks_FriendPublishers_PublisherId",
                table: "FriendBooks");

            migrationBuilder.RenameColumn(
                name: "PublisherId",
                table: "FriendBooks",
                newName: "FriendPublisherId");

            migrationBuilder.RenameIndex(
                name: "IX_FriendBooks_PublisherId",
                table: "FriendBooks",
                newName: "IX_FriendBooks_FriendPublisherId");

            migrationBuilder.RenameColumn(
                name: "Bio",
                table: "FriendAuthors",
                newName: "FriendAuthorName");

            migrationBuilder.RenameColumn(
                name: "AuthorName",
                table: "FriendAuthors",
                newName: "FriendAuthorBio");

            migrationBuilder.AddForeignKey(
                name: "FK_FriendBooks_FriendPublishers_FriendPublisherId",
                table: "FriendBooks",
                column: "FriendPublisherId",
                principalTable: "FriendPublishers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
