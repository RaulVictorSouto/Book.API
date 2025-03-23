using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Book.Shared.Data.Migrations
{
    /// <inheritdoc />
    public partial class AdiciocaoDoAtributoTag : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "BookTags",
                table: "TblBook",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "BookTagsList",
                table: "TblBook",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "[]");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BookTags",
                table: "TblBook");

            migrationBuilder.DropColumn(
                name: "BookTagsList",
                table: "TblBook");
        }
    }
}
