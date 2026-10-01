using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Bison.Razor.Migrations
{
	/// <inheritdoc />
	public partial class InitialCreate : Migration
	{
		/// <inheritdoc />
		protected override void Up(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.CreateTable(
				name: "taxon",
				columns: table => new
				{
					TaxonId = table
						.Column<int>(type: "INTEGER", nullable: false)
						.Annotation("Sqlite:Autoincrement", true),
					vernacular_name = table.Column<string>(type: "TEXT", nullable: false),
					parent_id = table.Column<int>(type: "INTEGER", nullable: true),
				},
				constraints: table =>
				{
					table.PrimaryKey("PK_taxon", x => x.TaxonId);
					table.ForeignKey(
						name: "FK_taxon_taxon_parent_id",
						column: x => x.parent_id,
						principalTable: "taxon",
						principalColumn: "TaxonId"
					);
				}
			);

			migrationBuilder.CreateTable(
				name: "user",
				columns: table => new
				{
					user_id = table
						.Column<int>(type: "INTEGER", nullable: false)
						.Annotation("Sqlite:Autoincrement", true),
					username = table.Column<string>(type: "TEXT", nullable: false),
					email = table.Column<string>(type: "TEXT", nullable: false),
					pw_hash = table.Column<int>(type: "INTEGER", nullable: false),
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
					taxon_id = table.Column<int>(type: "INTEGER", nullable: true),
					text = table.Column<string>(type: "TEXT", nullable: false),
					timestamp = table.Column<double>(type: "REAL", nullable: false),
					author_id = table.Column<int>(type: "INTEGER", nullable: false),
				},
				constraints: table =>
				{
					table.PrimaryKey("PK_observation", x => x.observation_id);
					table.ForeignKey(
						name: "FK_observation_taxon_taxon_id",
						column: x => x.taxon_id,
						principalTable: "taxon",
						principalColumn: "TaxonId"
					);
					table.ForeignKey(
						name: "FK_observation_user_author_id",
						column: x => x.author_id,
						principalTable: "user",
						principalColumn: "user_id",
						onDelete: ReferentialAction.Cascade
					);
				}
			);

			migrationBuilder.CreateTable(
				name: "comment",
				columns: table => new
				{
					comment_id = table
						.Column<int>(type: "INTEGER", nullable: false)
						.Annotation("Sqlite:Autoincrement", true),
					observation_id = table.Column<int>(type: "INTEGER", nullable: false),
					text = table.Column<string>(type: "TEXT", nullable: false),
					timestamp = table.Column<double>(type: "REAL", nullable: false),
					author_id = table.Column<int>(type: "INTEGER", nullable: false),
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

			migrationBuilder.CreateTable(
				name: "proposal",
				columns: table => new
				{
					Id = table
						.Column<int>(type: "INTEGER", nullable: false)
						.Annotation("Sqlite:Autoincrement", true),
					observation_id = table.Column<int>(type: "INTEGER", nullable: false),
					taxon_id = table.Column<int>(type: "INTEGER", nullable: false),
					text = table.Column<string>(type: "TEXT", nullable: false),
					timestamp = table.Column<double>(type: "REAL", nullable: false),
					author_id = table.Column<int>(type: "INTEGER", nullable: false),
				},
				constraints: table =>
				{
					table.PrimaryKey("PK_proposal", x => x.Id);
					table.ForeignKey(
						name: "FK_proposal_observation_observation_id",
						column: x => x.observation_id,
						principalTable: "observation",
						principalColumn: "observation_id",
						onDelete: ReferentialAction.Cascade
					);
					table.ForeignKey(
						name: "FK_proposal_taxon_taxon_id",
						column: x => x.taxon_id,
						principalTable: "taxon",
						principalColumn: "TaxonId",
						onDelete: ReferentialAction.Cascade
					);
					table.ForeignKey(
						name: "FK_proposal_user_author_id",
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

			migrationBuilder.CreateIndex(
				name: "IX_observation_author_id",
				table: "observation",
				column: "author_id"
			);

			migrationBuilder.CreateIndex(
				name: "IX_observation_taxon_id",
				table: "observation",
				column: "taxon_id"
			);

			migrationBuilder.CreateIndex(
				name: "IX_proposal_author_id",
				table: "proposal",
				column: "author_id"
			);

			migrationBuilder.CreateIndex(
				name: "IX_proposal_observation_id",
				table: "proposal",
				column: "observation_id"
			);

			migrationBuilder.CreateIndex(
				name: "IX_proposal_taxon_id",
				table: "proposal",
				column: "taxon_id"
			);

			migrationBuilder.CreateIndex(
				name: "IX_taxon_parent_id",
				table: "taxon",
				column: "parent_id"
			);
		}

		/// <inheritdoc />
		protected override void Down(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.DropTable(name: "comment");

			migrationBuilder.DropTable(name: "proposal");

			migrationBuilder.DropTable(name: "observation");

			migrationBuilder.DropTable(name: "taxon");

			migrationBuilder.DropTable(name: "user");
		}
	}
}
