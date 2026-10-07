using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Bison.Razor.Migrations
{
	/// <inheritdoc />
	public partial class AddUserUniqueConstraints : Migration
	{
		/// <inheritdoc />
		protected override void Up(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.AlterColumn<string>(
				name: "username",
				table: "user",
				type: "TEXT",
				nullable: false,
				collation: "NOCASE",
				oldClrType: typeof(string),
				oldType: "TEXT"
			);

			migrationBuilder.AlterColumn<string>(
				name: "pw_hash",
				table: "user",
				type: "TEXT",
				nullable: false,
				oldClrType: typeof(int),
				oldType: "INTEGER"
			);

			migrationBuilder.AlterColumn<string>(
				name: "email",
				table: "user",
				type: "TEXT",
				nullable: false,
				collation: "NOCASE",
				oldClrType: typeof(string),
				oldType: "TEXT"
			);

			migrationBuilder.CreateIndex(
				name: "IX_user_email",
				table: "user",
				column: "email",
				unique: true
			);

			migrationBuilder.CreateIndex(
				name: "IX_user_username",
				table: "user",
				column: "username",
				unique: true
			);
		}

		/// <inheritdoc />
		protected override void Down(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.DropIndex(name: "IX_user_email", table: "user");

			migrationBuilder.DropIndex(name: "IX_user_username", table: "user");

			migrationBuilder.AlterColumn<string>(
				name: "username",
				table: "user",
				type: "TEXT",
				nullable: false,
				oldClrType: typeof(string),
				oldType: "TEXT",
				oldCollation: "NOCASE"
			);

			migrationBuilder.AlterColumn<int>(
				name: "pw_hash",
				table: "user",
				type: "INTEGER",
				nullable: false,
				oldClrType: typeof(string),
				oldType: "TEXT"
			);

			migrationBuilder.AlterColumn<string>(
				name: "email",
				table: "user",
				type: "TEXT",
				nullable: false,
				oldClrType: typeof(string),
				oldType: "TEXT",
				oldCollation: "NOCASE"
			);
		}
	}
}
