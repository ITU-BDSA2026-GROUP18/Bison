using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Bison.Razor.Migrations
{
    /// <inheritdoc />
    public partial class AddProposalsAndTaxon : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "taxon_id",
                table: "observation",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "taxon",
                columns: table => new
                {
                    taxon_id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    parent_taxon_id = table.Column<int>(type: "INTEGER", nullable: true),
                    dwc_taxon_id = table.Column<string>(type: "TEXT", nullable: false),
                    vernacular_name = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_taxon", x => x.taxon_id);
                    table.ForeignKey(
                        name: "FK_taxon_taxon_parent_taxon_id",
                        column: x => x.parent_taxon_id,
                        principalTable: "taxon",
                        principalColumn: "taxon_id");
                });

            migrationBuilder.CreateTable(
                name: "proposal",
                columns: table => new
                {
                    proposal_id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    author_id = table.Column<int>(type: "INTEGER", nullable: false),
                    observation_id = table.Column<int>(type: "INTEGER", nullable: false),
                    taxon_id = table.Column<int>(type: "INTEGER", nullable: false),
                    text = table.Column<string>(type: "TEXT", nullable: false),
                    pub_date = table.Column<long>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_proposal", x => x.proposal_id);
                    table.ForeignKey(
                        name: "FK_proposal_observation_observation_id",
                        column: x => x.observation_id,
                        principalTable: "observation",
                        principalColumn: "observation_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_proposal_taxon_taxon_id",
                        column: x => x.taxon_id,
                        principalTable: "taxon",
                        principalColumn: "taxon_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_proposal_user_author_id",
                        column: x => x.author_id,
                        principalTable: "user",
                        principalColumn: "user_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_observation_taxon_id",
                table: "observation",
                column: "taxon_id");

            migrationBuilder.CreateIndex(
                name: "IX_proposal_author_id",
                table: "proposal",
                column: "author_id");

            migrationBuilder.CreateIndex(
                name: "IX_proposal_observation_id",
                table: "proposal",
                column: "observation_id");

            migrationBuilder.CreateIndex(
                name: "IX_proposal_taxon_id",
                table: "proposal",
                column: "taxon_id");

            migrationBuilder.CreateIndex(
                name: "IX_taxon_parent_taxon_id",
                table: "taxon",
                column: "parent_taxon_id");

            migrationBuilder.AddForeignKey(
                name: "FK_observation_taxon_taxon_id",
                table: "observation",
                column: "taxon_id",
                principalTable: "taxon",
                principalColumn: "taxon_id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_observation_taxon_taxon_id",
                table: "observation");

            migrationBuilder.DropTable(
                name: "proposal");

            migrationBuilder.DropTable(
                name: "taxon");

            migrationBuilder.DropIndex(
                name: "IX_observation_taxon_id",
                table: "observation");

            migrationBuilder.DropColumn(
                name: "taxon_id",
                table: "observation");
        }
    }
}
