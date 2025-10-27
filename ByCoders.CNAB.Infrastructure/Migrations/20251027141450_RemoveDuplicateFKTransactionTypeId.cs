using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ByCoders.CNAB.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RemoveDuplicateFKTransactionTypeId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_transactions_transactiontypes_transactiontypeid1",
                table: "transactions");

            migrationBuilder.DropIndex(
                name: "ix_transactions_transactiontypeid1",
                table: "transactions");

            migrationBuilder.DropColumn(
                name: "transactiontypeid1",
                table: "transactions");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "transactiontypeid1",
                table: "transactions",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "ix_transactions_transactiontypeid1",
                table: "transactions",
                column: "transactiontypeid1");

            migrationBuilder.AddForeignKey(
                name: "fk_transactions_transactiontypes_transactiontypeid1",
                table: "transactions",
                column: "transactiontypeid1",
                principalTable: "transactiontypes",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
