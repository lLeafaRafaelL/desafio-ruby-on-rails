using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ByCoders.CNAB.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RemovingUniqueConstraintFromStoreName : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "ix_transactions_store_name",
                table: "transactions");

            migrationBuilder.CreateIndex(
                name: "ix_transactions_store_name",
                table: "transactions",
                column: "store_name");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "ix_transactions_store_name",
                table: "transactions");

            migrationBuilder.CreateIndex(
                name: "ix_transactions_store_name",
                table: "transactions",
                column: "store_name",
                unique: true);
        }
    }
}
