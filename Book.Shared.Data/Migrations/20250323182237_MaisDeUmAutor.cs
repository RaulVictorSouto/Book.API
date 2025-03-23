using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Book.Shared.Data.Migrations
{
    /// <inheritdoc />
    public partial class MaisDeUmAutor : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TblBook_TblAuthor_AuthorID",
                table: "TblBook");

            migrationBuilder.DropIndex(
                name: "IX_TblBook_AuthorID",
                table: "TblBook");

            migrationBuilder.DropColumn(
                name: "AuthorID",
                table: "TblBook");

            migrationBuilder.CreateTable(
                name: "TblBookAuthors",
                columns: table => new
                {
                    AuthorsAuthorID = table.Column<int>(type: "int", nullable: false),
                    BooksBookID = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TblBookAuthors", x => new { x.AuthorsAuthorID, x.BooksBookID });
                    table.ForeignKey(
                        name: "FK_TblBookAuthors_TblAuthor_AuthorsAuthorID",
                        column: x => x.AuthorsAuthorID,
                        principalTable: "TblAuthor",
                        principalColumn: "AuthorID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TblBookAuthors_TblBook_BooksBookID",
                        column: x => x.BooksBookID,
                        principalTable: "TblBook",
                        principalColumn: "BookID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TblBookAuthors_BooksBookID",
                table: "TblBookAuthors",
                column: "BooksBookID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TblBookAuthors");

            migrationBuilder.AddColumn<int>(
                name: "AuthorID",
                table: "TblBook",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_TblBook_AuthorID",
                table: "TblBook",
                column: "AuthorID");

            migrationBuilder.AddForeignKey(
                name: "FK_TblBook_TblAuthor_AuthorID",
                table: "TblBook",
                column: "AuthorID",
                principalTable: "TblAuthor",
                principalColumn: "AuthorID",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
