using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Bison.Razor.Migrations
{
    /// <inheritdoc />
    public partial class AddCommentReplies : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "parent_comment_id",
                table: "comment",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_comment_parent_comment_id",
                table: "comment",
                column: "parent_comment_id");

            migrationBuilder.AddForeignKey(
                name: "FK_comment_comment_parent_comment_id",
                table: "comment",
                column: "parent_comment_id",
                principalTable: "comment",
                principalColumn: "post_id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_comment_comment_parent_comment_id",
                table: "comment");

            migrationBuilder.DropIndex(
                name: "IX_comment_parent_comment_id",
                table: "comment");

            migrationBuilder.DropColumn(
                name: "parent_comment_id",
                table: "comment");
        }
    }
}
