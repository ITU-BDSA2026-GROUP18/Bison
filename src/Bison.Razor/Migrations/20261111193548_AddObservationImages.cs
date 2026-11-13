using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Bison.Razor.Migrations
{
	/// <inheritdoc />
	public partial class AddObservationImages : Migration
	{
		/// <inheritdoc />
		protected override void Up(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.AddColumn<byte[]>(
				name: "image",
				table: "observation",
				type: "BLOB",
				nullable: true
			);

			migrationBuilder.AddColumn<string>(
				name: "image_content_type",
				table: "observation",
				type: "TEXT",
				nullable: true
			);
		}

		/// <inheritdoc />
		protected override void Down(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.DropColumn(name: "image", table: "observation");
			migrationBuilder.DropColumn(name: "image_content_type", table: "observation");
		}
	}
}
