using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HomeLibrary.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddCreatedAtDefault : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                "ALTER TABLE books ALTER COLUMN created_at SET DEFAULT NOW();");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                "ALTER TABLE books ALTER COLUMN created_at DROP DEFAULT;");
        }
    }
}
