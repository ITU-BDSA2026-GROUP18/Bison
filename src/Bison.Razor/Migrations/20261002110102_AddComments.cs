using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Bison.Razor.Migrations
{
	/// <inheritdoc />
	public partial class AddComments : Migration
	{
		/// <inheritdoc />
		protected override void Up(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.CreateTable(
				name: "comment",
				columns: table => new
				{
					comment_id = table
						.Column<int>(type: "INTEGER", nullable: false)
						.Annotation("Sqlite:Autoincrement", true),
					observation_id = table.Column<int>(type: "INTEGER", nullable: false),
					author_id = table.Column<int>(type: "INTEGER", nullable: false),
					comment = table.Column<string>(type: "TEXT", nullable: false),
					pub_date = table.Column<long>(type: "INTEGER", nullable: false),
				},
				constraints: table =>
				{
					table.PrimaryKey("PK_comment", x => x.comment_id);
					table.ForeignKey(
						name: "FK_comment_observation_observation_id",
						column: x => x.observation_id,
						principalTable: "observation",
						principalColumn: "observation_id",
						onDelete: ReferentialAction.Cascade
					);
					table.ForeignKey(
						name: "FK_comment_user_author_id",
						column: x => x.author_id,
						principalTable: "user",
						principalColumn: "user_id",
						onDelete: ReferentialAction.Cascade
					);
				}
			);

			migrationBuilder.CreateIndex(
				name: "IX_comment_author_id",
				table: "comment",
				column: "author_id"
			);

			migrationBuilder.CreateIndex(
				name: "IX_comment_observation_id",
				table: "comment",
				column: "observation_id"
			);
		}

		/// <inheritdoc />
		protected override void Down(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.DropTable(name: "comment");
		}
	}
}
