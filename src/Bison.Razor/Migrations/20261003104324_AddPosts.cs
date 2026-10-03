using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Bison.Razor.Migrations
{
    /// <inheritdoc />
    public partial class AddPosts : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_comment_user_author_id",
                table: "comment");

            migrationBuilder.DropForeignKey(
                name: "FK_observation_user_author_id",
                table: "observation");

            migrationBuilder.DropForeignKey(
                name: "FK_proposal_user_author_id",
                table: "proposal");

            migrationBuilder.DropIndex(
                name: "IX_proposal_author_id",
                table: "proposal");

            migrationBuilder.DropIndex(
                name: "IX_observation_author_id",
                table: "observation");

            migrationBuilder.DropIndex(
                name: "IX_comment_author_id",
                table: "comment");

            migrationBuilder.DropColumn(
                name: "author_id",
                table: "proposal");

            migrationBuilder.DropColumn(
                name: "pub_date",
                table: "proposal");

            migrationBuilder.DropColumn(
                name: "text",
                table: "proposal");

            migrationBuilder.DropColumn(
                name: "author_id",
                table: "observation");

            migrationBuilder.DropColumn(
                name: "pub_date",
                table: "observation");

            migrationBuilder.DropColumn(
                name: "text",
                table: "observation");

            migrationBuilder.DropColumn(
                name: "author_id",
                table: "comment");

            migrationBuilder.DropColumn(
                name: "comment",
                table: "comment");

            migrationBuilder.DropColumn(
                name: "pub_date",
                table: "comment");

            migrationBuilder.RenameColumn(
                name: "proposal_id",
                table: "proposal",
                newName: "post_id");

            migrationBuilder.RenameColumn(
                name: "observation_id",
                table: "observation",
                newName: "post_id");

            migrationBuilder.RenameColumn(
                name: "comment_id",
                table: "comment",
                newName: "post_id");

            migrationBuilder.CreateTable(
                name: "post",
                columns: table => new
                {
                    post_id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    author_id = table.Column<int>(type: "INTEGER", nullable: false),
                    text = table.Column<string>(type: "TEXT", nullable: false),
                    pub_date = table.Column<long>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_post", x => x.post_id);
                    table.ForeignKey(
                        name: "FK_post_user_author_id",
                        column: x => x.author_id,
                        principalTable: "user",
                        principalColumn: "user_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_post_author_id",
                table: "post",
                column: "author_id");

            migrationBuilder.AddForeignKey(
                name: "FK_comment_post_post_id",
                table: "comment",
                column: "post_id",
                principalTable: "post",
                principalColumn: "post_id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_observation_post_post_id",
                table: "observation",
                column: "post_id",
                principalTable: "post",
                principalColumn: "post_id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_proposal_post_post_id",
                table: "proposal",
                column: "post_id",
                principalTable: "post",
                principalColumn: "post_id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_comment_post_post_id",
                table: "comment");

            migrationBuilder.DropForeignKey(
                name: "FK_observation_post_post_id",
                table: "observation");

            migrationBuilder.DropForeignKey(
                name: "FK_proposal_post_post_id",
                table: "proposal");

            migrationBuilder.DropTable(
                name: "post");

            migrationBuilder.RenameColumn(
                name: "post_id",
                table: "proposal",
                newName: "proposal_id");

            migrationBuilder.RenameColumn(
                name: "post_id",
                table: "observation",
                newName: "observation_id");

            migrationBuilder.RenameColumn(
                name: "post_id",
                table: "comment",
                newName: "comment_id");

            migrationBuilder.AddColumn<int>(
                name: "author_id",
                table: "proposal",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<long>(
                name: "pub_date",
                table: "proposal",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<string>(
                name: "text",
                table: "proposal",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "author_id",
                table: "observation",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<double>(
                name: "pub_date",
                table: "observation",
                type: "REAL",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<string>(
                name: "text",
                table: "observation",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "author_id",
                table: "comment",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "comment",
                table: "comment",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<long>(
                name: "pub_date",
                table: "comment",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.CreateIndex(
                name: "IX_proposal_author_id",
                table: "proposal",
                column: "author_id");

            migrationBuilder.CreateIndex(
                name: "IX_observation_author_id",
                table: "observation",
                column: "author_id");

            migrationBuilder.CreateIndex(
                name: "IX_comment_author_id",
                table: "comment",
                column: "author_id");

            migrationBuilder.AddForeignKey(
                name: "FK_comment_user_author_id",
                table: "comment",
                column: "author_id",
                principalTable: "user",
                principalColumn: "user_id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_observation_user_author_id",
                table: "observation",
                column: "author_id",
                principalTable: "user",
                principalColumn: "user_id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_proposal_user_author_id",
                table: "proposal",
                column: "author_id",
                principalTable: "user",
                principalColumn: "user_id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
