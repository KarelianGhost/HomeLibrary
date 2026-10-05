using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HomeLibrary.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddStoredFunctions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
        CREATE OR REPLACE FUNCTION sp_books_select(
            p_search_term TEXT DEFAULT NULL,
            p_author_id   INT DEFAULT NULL,
            p_year_from   INT DEFAULT NULL,
            p_year_to     INT DEFAULT NULL
        )
        RETURNS TABLE (
            book_id INT, title VARCHAR, author TEXT,
            publish_year INT, publisher VARCHAR, isbn VARCHAR, pages INT
        ) AS $$
        BEGIN
            RETURN QUERY
            SELECT b.book_id, b.title,
                   a.last_name || ' ' || a.first_name,
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
        CREATE OR REPLACE FUNCTION sp_books_insert(
            p_title VARCHAR, p_author_id INT, p_publish_year INT,
            p_publisher VARCHAR DEFAULT NULL, p_isbn VARCHAR DEFAULT NULL,
            p_pages INT DEFAULT NULL, p_contents XML DEFAULT NULL
        )
        RETURNS INT AS $$
        DECLARE new_id INT;
        BEGIN
            INSERT INTO books (title, author_id, publish_year,
                               publisher, isbn, pages, contents)
            VALUES (p_title, p_author_id, p_publish_year,
                    p_publisher, p_isbn, p_pages, p_contents)
            RETURNING book_id INTO new_id;
            RETURN new_id;
        END;
        $$ LANGUAGE plpgsql;
    ");

            migrationBuilder.Sql(@"
        CREATE OR REPLACE FUNCTION sp_books_update(
            p_book_id INT, p_title VARCHAR, p_author_id INT,
            p_publish_year INT, p_publisher VARCHAR DEFAULT NULL,
            p_isbn VARCHAR DEFAULT NULL, p_pages INT DEFAULT NULL,
            p_contents XML DEFAULT NULL
        )
        RETURNS INT AS $$
        DECLARE affected INT;
        BEGIN
            UPDATE books
            SET title = p_title, author_id = p_author_id,
                publish_year = p_publish_year, publisher = p_publisher,
                isbn = p_isbn, pages = p_pages, contents = p_contents,
                updated_at = NOW()
            WHERE book_id = p_book_id;
            GET DIAGNOSTICS affected = ROW_COUNT;
            RETURN affected;
        END;
        $$ LANGUAGE plpgsql;
    ");

            migrationBuilder.Sql(@"
        CREATE OR REPLACE FUNCTION sp_books_delete(p_book_id INT)
        RETURNS INT AS $$
        DECLARE affected INT;
        BEGIN
            DELETE FROM books WHERE book_id = p_book_id;
            GET DIAGNOSTICS affected = ROW_COUNT;
            RETURN affected;
        END;
        $$ LANGUAGE plpgsql;
    ");

            migrationBuilder.Sql(@"
        CREATE OR REPLACE FUNCTION sp_books_search_in_contents(p_keyword TEXT)
        RETURNS TABLE (book_id INT, title VARCHAR, publish_year INT) AS $$
        BEGIN
            RETURN QUERY
            SELECT b.book_id, b.title, b.publish_year
            FROM books b
            WHERE xpath_exists('//*[contains(., $1)]', b.contents, ARRAY[p_keyword]);
        END;
        $$ LANGUAGE plpgsql;
    ");

            // Авторы
            migrationBuilder.Sql(@"
        CREATE OR REPLACE FUNCTION sp_authors_select()
        RETURNS TABLE (author_id INT, full_name TEXT) AS $$
        BEGIN
            RETURN QUERY
            SELECT a.author_id, a.last_name || ' ' || a.first_name
            FROM authors a ORDER BY a.last_name, a.first_name;
        END;
        $$ LANGUAGE plpgsql;
    ");

            migrationBuilder.Sql(@"
        CREATE OR REPLACE FUNCTION sp_authors_insert(
            p_first_name VARCHAR, p_last_name VARCHAR
        )
        RETURNS INT AS $$
        DECLARE new_id INT;
        BEGIN
            INSERT INTO authors (first_name, last_name)
            VALUES (p_first_name, p_last_name)
            RETURNING author_id INTO new_id;
            RETURN new_id;
        END;
        $$ LANGUAGE plpgsql;
    ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DROP FUNCTION IF EXISTS sp_authors_insert(VARCHAR, VARCHAR);");
            migrationBuilder.Sql("DROP FUNCTION IF EXISTS sp_authors_select();");
            migrationBuilder.Sql("DROP FUNCTION IF EXISTS sp_books_search_in_contents(TEXT);");
            migrationBuilder.Sql("DROP FUNCTION IF EXISTS sp_books_delete(INT);");
            migrationBuilder.Sql("DROP FUNCTION IF EXISTS sp_books_update(INT, VARCHAR, INT, INT, VARCHAR, VARCHAR, INT, XML);");
            migrationBuilder.Sql("DROP FUNCTION IF EXISTS sp_books_insert(VARCHAR, INT, INT, VARCHAR, VARCHAR, INT, XML);");
            migrationBuilder.Sql("DROP FUNCTION IF EXISTS sp_books_select(TEXT, INT, INT, INT);");
        }
    }
}
