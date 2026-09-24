using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Bison.Razor.Migrations
{
	/// <inheritdoc />
	public partial class InitialDBSchema : Migration
	{
		/// <inheritdoc />
		protected override void Up(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.CreateTable(
				name: "user",
				columns: table => new
				{
					user_id = table
						.Column<int>(type: "INTEGER", nullable: false)
						.Annotation("Sqlite:Autoincrement", true),
					username = table.Column<string>(type: "TEXT", nullable: false),
					email = table.Column<string>(type: "TEXT", nullable: false),
				},
				constraints: table =>
				{
					table.PrimaryKey("PK_user", x => x.user_id);
				}
			);

			migrationBuilder.CreateTable(
				name: "observation",
				columns: table => new
				{
					observation_id = table
						.Column<int>(type: "INTEGER", nullable: false)
						.Annotation("Sqlite:Autoincrement", true),
					author_id = table.Column<int>(type: "INTEGER", nullable: false),
					text = table.Column<string>(type: "TEXT", nullable: false),
					pub_date = table.Column<double>(type: "REAL", nullable: false),
				},
				constraints: table =>
				{
					table.PrimaryKey("PK_observation", x => x.observation_id);
					table.ForeignKey(
						name: "FK_observation_user_author_id",
						column: x => x.author_id,
						principalTable: "user",
						principalColumn: "user_id",
						onDelete: ReferentialAction.Cascade
					);
				}
			);

			migrationBuilder.CreateIndex(
				name: "IX_observation_author_id",
				table: "observation",
				column: "author_id"
			);
		}

		/// <inheritdoc />
		protected override void Down(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.DropTable(name: "observation");

			migrationBuilder.DropTable(name: "user");
		}
	}
}
