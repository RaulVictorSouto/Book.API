using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Book.Shared.Data.Migrations
{
    /// <inheritdoc />
    public partial class EsqueciDoNoMapped : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BookTagsList",
                table: "TblBook");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "BookTagsList",
                table: "TblBook",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
