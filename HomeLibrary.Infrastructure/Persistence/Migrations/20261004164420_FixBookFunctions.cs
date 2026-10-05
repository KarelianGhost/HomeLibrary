using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HomeLibrary.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class FixBookFunctions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DROP FUNCTION IF EXISTS sp_books_select(TEXT, INT, INT, INT);");
            migrationBuilder.Sql("DROP FUNCTION IF EXISTS sp_books_search_in_contents(TEXT);");

            migrationBuilder.Sql(@"
        CREATE OR REPLACE FUNCTION sp_books_select(
            p_search_term TEXT DEFAULT NULL,
            p_author_id   INT DEFAULT NULL,
            p_year_from   INT DEFAULT NULL,
            p_year_to     INT DEFAULT NULL
        )
        RETURNS TABLE (
            book_id            INT,
            title              VARCHAR,
            author_id          INT,
            author_first_name  VARCHAR,
            author_last_name   VARCHAR,
            publish_year       INT,
            publisher          VARCHAR,
            isbn               VARCHAR,
            pages              INT
        ) AS $$
        BEGIN
            RETURN QUERY
            SELECT b.book_id, b.title, b.author_id,
                   a.first_name, a.last_name,
                   b.publish_year, b.publisher, b.isbn, b.pages
            FROM books b
            JOIN authors a ON a.author_id = b.author_id
            WHERE (p_author_id IS NULL OR b.author_id = p_author_id)
              AND (p_year_from IS NULL OR b.publish_year >= p_year_from)
              AND (p_year_to   IS NULL OR b.publish_year <= p_year_to)
              AND (p_search_term IS NULL
                   OR b.search_vector @@ plainto_tsquery('russian', p_search_term));
        END;
        $$ LANGUAGE plpgsql;
    ");

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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DROP FUNCTION IF EXISTS sp_books_select(TEXT, INT, INT, INT);");
            migrationBuilder.Sql("DROP FUNCTION IF EXISTS sp_books_search_in_contents(TEXT);");

            migrationBuilder.Sql(@"
        CREATE OR REPLACE FUNCTION sp_books_select(
            p_search_term TEXT DEFAULT NULL,
            p_author_id   INT DEFAULT NULL,
            p_year_from   INT DEFAULT NULL,
            p_year_to     INT DEFAULT NULL
        )
        RETURNS TABLE (
            book_id      INT,
            title        VARCHAR,
            author       TEXT,
            publish_year INT,
            publisher    VARCHAR,
            isbn         VARCHAR,
            pages        INT
        ) AS $$
        BEGIN
            RETURN QUERY
            SELECT b.book_id, b.title,
                   a.last_name || ' ' || a.first_name AS author,
                   b.publish_year, b.publisher, b.isbn, b.pages
            FROM books b
            JOIN authors a ON a.author_id = b.author_id
            WHERE (p_author_id IS NULL OR b.author_id = p_author_id)
              AND (p_year_from IS NULL OR b.publish_year >= p_year_from)
              AND (p_year_to   IS NULL OR b.publish_year <= p_year_to)
              AND (p_search_term IS NULL
                   OR b.search_vector @@ plainto_tsquery('russian', p_search_term));
        END;
        $$ LANGUAGE plpgsql;
    ");

            migrationBuilder.Sql(@"
        CREATE OR REPLACE FUNCTION sp_books_search_in_contents(p_keyword TEXT)
        RETURNS TABLE (
            book_id      INT,
            title        VARCHAR,
            publish_year INT
        ) AS $$
        BEGIN
            RETURN QUERY
            SELECT b.book_id, b.title, b.publish_year
            FROM books b
            WHERE xpath_exists('//*[contains(., $1)]', b.contents, ARRAY[p_keyword]);
        END;
        $$ LANGUAGE plpgsql;
    ");
        }
    }
}
