using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AnonymousChatApi.Migrations
{
    /// <inheritdoc />
    public partial class AddIsDeleteFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LastReadMessageId",
                schema: "public",
                table: "Users");

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                schema: "public",
                table: "MessageBases",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<long>(
                name: "LastReadMessageId",
                schema: "public",
                table: "ChatUsers",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                schema: "public",
                table: "Chats",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateIndex(
                name: "IX_ChatUsers_LastReadMessageId",
                schema: "public",
                table: "ChatUsers",
                column: "LastReadMessageId");

            migrationBuilder.AddForeignKey(
                name: "FK_ChatUsers_MessageBases_LastReadMessageId",
                schema: "public",
                table: "ChatUsers",
                column: "LastReadMessageId",
                principalSchema: "public",
                principalTable: "MessageBases",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ChatUsers_MessageBases_LastReadMessageId",
                schema: "public",
                table: "ChatUsers");

            migrationBuilder.DropIndex(
                name: "IX_ChatUsers_LastReadMessageId",
                schema: "public",
                table: "ChatUsers");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                schema: "public",
                table: "MessageBases");

            migrationBuilder.DropColumn(
                name: "LastReadMessageId",
                schema: "public",
                table: "ChatUsers");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                schema: "public",
                table: "Chats");

            migrationBuilder.AddColumn<long>(
                name: "LastReadMessageId",
                schema: "public",
                table: "Users",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);
        }
    }
}
