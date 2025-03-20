using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Book.Shared.Data.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TblAuthor",
                columns: table => new
                {
                    AuthorID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AuthorName = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TblAuthor", x => x.AuthorID);
                });

            migrationBuilder.CreateTable(
                name: "TblGenre",
                columns: table => new
                {
                    GenreID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    GenreName = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TblGenre", x => x.GenreID);
                });

            migrationBuilder.CreateTable(
                name: "TblBook",
                columns: table => new
                {
                    BookID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    BookTitle = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    BookLanguage = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    BookPublisher = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    BookISBN = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    BookRating = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    BookCoverPage = table.Column<byte[]>(type: "varbinary(max)", nullable: false),
                    AuthorID = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TblBook", x => x.BookID);
                    table.ForeignKey(
                        name: "FK_TblBook_TblAuthor_AuthorID",
                        column: x => x.AuthorID,
                        principalTable: "TblAuthor",
                        principalColumn: "AuthorID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TblBookGenres",
                columns: table => new
                {
                    BooksBookID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    GenresGenreID = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TblBookGenres", x => new { x.BooksBookID, x.GenresGenreID });
                    table.ForeignKey(
                        name: "FK_TblBookGenres_TblBook_BooksBookID",
                        column: x => x.BooksBookID,
                        principalTable: "TblBook",
                        principalColumn: "BookID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TblBookGenres_TblGenre_GenresGenreID",
                        column: x => x.GenresGenreID,
                        principalTable: "TblGenre",
                        principalColumn: "GenreID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TblBook_AuthorID",
                table: "TblBook",
                column: "AuthorID");

            migrationBuilder.CreateIndex(
                name: "IX_TblBookGenres_GenresGenreID",
                table: "TblBookGenres",
                column: "GenresGenreID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TblBookGenres");

            migrationBuilder.DropTable(
                name: "TblBook");

            migrationBuilder.DropTable(
                name: "TblGenre");

            migrationBuilder.DropTable(
                name: "TblAuthor");
        }
    }
}
