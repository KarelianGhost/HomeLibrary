using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HomeLibrary.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddFullTextSearch : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                "ALTER TABLE books ADD COLUMN search_vector tsvector;");

            migrationBuilder.Sql(
                "CREATE INDEX idx_books_search ON books USING GIN(search_vector);");

            migrationBuilder.Sql(@"
            CREATE OR REPLACE FUNCTION books_search_trigger() RETURNS trigger AS $$
            BEGIN
                NEW.search_vector :=
                    setweight(to_tsvector('russian', COALESCE(NEW.title, '')), 'A') ||
                    setweight(to_tsvector('russian', COALESCE(
                        (SELECT last_name || ' ' || first_name
                         FROM authors WHERE author_id = NEW.author_id), ''
                    )), 'B');
                RETURN NEW;
            END;
            $$ LANGUAGE plpgsql;
        ");

            migrationBuilder.Sql(@"
            CREATE TRIGGER trg_books_search
                BEFORE INSERT OR UPDATE ON books
                FOR EACH ROW EXECUTE FUNCTION books_search_trigger();
        ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DROP TRIGGER IF EXISTS trg_books_search ON books;");
            migrationBuilder.Sql("DROP FUNCTION IF EXISTS books_search_trigger();");
            migrationBuilder.Sql("DROP INDEX IF EXISTS idx_books_search;");
            migrationBuilder.Sql("ALTER TABLE books DROP COLUMN IF EXISTS search_vector;");
        }
    }
}
