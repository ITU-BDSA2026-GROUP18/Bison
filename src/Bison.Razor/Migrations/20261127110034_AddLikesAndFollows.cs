using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Bison.Razor.Migrations
{
	/// <inheritdoc />
	public partial class AddLikesAndFollows : Migration
	{
		/// <inheritdoc />
		protected override void Up(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.AddColumn<int>(
				name: "likes",
				table: "observation",
				type: "INTEGER",
				nullable: false,
				defaultValue: 0
			);

			migrationBuilder.CreateTable(
				name: "follow",
				columns: table => new
				{
					follower_id = table.Column<string>(type: "TEXT", nullable: false),
					followee_id = table.Column<string>(type: "TEXT", nullable: false),
				},
				constraints: table =>
				{
					table.PrimaryKey("PK_follow", x => new { x.follower_id, x.followee_id });
					table.CheckConstraint("CK_follow_not_self", "follower_id <> followee_id");
					table.ForeignKey(
						name: "FK_follow_user_followee_id",
						column: x => x.followee_id,
						principalTable: "user",
						principalColumn: "user_id",
						onDelete: ReferentialAction.Cascade
					);
					table.ForeignKey(
						name: "FK_follow_user_follower_id",
						column: x => x.follower_id,
						principalTable: "user",
						principalColumn: "user_id",
						onDelete: ReferentialAction.Cascade
					);
				}
			);

			migrationBuilder.CreateTable(
				name: "observation_like",
				columns: table => new
				{
					user_id = table.Column<string>(type: "TEXT", nullable: false),
					observation_id = table.Column<int>(type: "INTEGER", nullable: false),
				},
				constraints: table =>
				{
					table.PrimaryKey(
						"PK_observation_like",
						x => new { x.user_id, x.observation_id }
					);
					table.ForeignKey(
						name: "FK_observation_like_observation_observation_id",
						column: x => x.observation_id,
						principalTable: "observation",
						principalColumn: "post_id",
						onDelete: ReferentialAction.Cascade
					);
					table.ForeignKey(
						name: "FK_observation_like_user_user_id",
						column: x => x.user_id,
						principalTable: "user",
						principalColumn: "user_id",
						onDelete: ReferentialAction.Cascade
					);
				}
			);

			migrationBuilder.CreateIndex(
				name: "IX_follow_followee_id",
				table: "follow",
				column: "followee_id"
			);

			migrationBuilder.CreateIndex(
				name: "IX_observation_like_observation_id",
				table: "observation_like",
				column: "observation_id"
			);
		}

		/// <inheritdoc />
		protected override void Down(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.DropTable(name: "follow");

			migrationBuilder.DropTable(name: "observation_like");

			migrationBuilder.DropColumn(name: "likes", table: "observation");
		}
	}
}
