using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HomeLibrary.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class FixSearchInContents : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DROP FUNCTION IF EXISTS sp_books_search_in_contents(TEXT);");

            migrationBuilder.Sql(@"
        CREATE OR REPLACE FUNCTION sp_books_search_in_contents(p_keyword TEXT)
        RETURNS TABLE (
            book_id            INT,
            title              VARCHAR,
            author_id          INT,
            author_first_name  VARCHAR,
            author_last_name   VARCHAR,
            publish_year       INT
        ) AS $$
        BEGIN
            RETURN QUERY
            SELECT b.book_id, b.title, b.author_id,
                   a.first_name, a.last_name, b.publish_year
            FROM books b
            JOIN authors a ON a.author_id = b.author_id
            WHERE b.contents IS NOT NULL
              AND CAST(b.contents AS TEXT) ILIKE '%' || p_keyword || '%';
        END;
        $$ LANGUAGE plpgsql;
    ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DROP FUNCTION IF EXISTS sp_books_search_in_contents(TEXT);");

            migrationBuilder.Sql(@"
        CREATE OR REPLACE FUNCTION sp_books_search_in_contents(p_keyword TEXT)
        RETURNS TABLE (
            book_id            INT,
            title              VARCHAR,
            author_id          INT,
            author_first_name  VARCHAR,
            author_last_name   VARCHAR,
            publish_year       INT
        ) AS $$
        BEGIN
            RETURN QUERY
            SELECT b.book_id, b.title, b.author_id,
                   a.first_name, a.last_name, b.publish_year
            FROM books b
            JOIN authors a ON a.author_id = b.author_id
            WHERE xpath_exists('//*[contains(., $1)]', b.contents, ARRAY[p_keyword]);
        END;
        $$ LANGUAGE plpgsql;
    ");
        }
    }
}
