using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OnlineSellingStore.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class AddFriendBookInDb : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "FriendPublishers",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PublisherName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Address = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Contact = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FriendPublishers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "FriendBooks",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    BookName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    BookDescription = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    BookImage = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Price = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Rating = table.Column<int>(type: "int", nullable: false),
                    FriendPublisherId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FriendBooks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FriendBooks_FriendPublishers_FriendPublisherId",
                        column: x => x.FriendPublisherId,
                        principalTable: "FriendPublishers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "FriendAuthors",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FriendAuthorName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FriendAuthorBio = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FriendBookId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FriendAuthors", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FriendAuthors_FriendBooks_FriendBookId",
                        column: x => x.FriendBookId,
                        principalTable: "FriendBooks",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_FriendAuthors_FriendBookId",
                table: "FriendAuthors",
                column: "FriendBookId");

            migrationBuilder.CreateIndex(
                name: "IX_FriendBooks_FriendPublisherId",
                table: "FriendBooks",
                column: "FriendPublisherId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FriendAuthors");

            migrationBuilder.DropTable(
                name: "FriendBooks");

            migrationBuilder.DropTable(
                name: "FriendPublishers");
        }
    }
}
