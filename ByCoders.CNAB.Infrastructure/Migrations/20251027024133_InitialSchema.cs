using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace ByCoders.CNAB.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialSchema : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "cnabfiles",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    filename = table.Column<string>(type: "varchar", maxLength: 255, nullable: false),
                    filepath = table.Column<string>(type: "varchar", maxLength: 500, nullable: false),
                    filesize = table.Column<long>(type: "bigint", nullable: false),
                    uploadedon = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    processingstartedon = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    processedon = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    failedon = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    errormessage = table.Column<string>(type: "varchar", maxLength: 2000, nullable: true),
                    transactioncount = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_cnabfiles", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "transactiontypes",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false),
                    description = table.Column<string>(type: "varchar", maxLength: 30, nullable: false),
                    nature = table.Column<byte>(type: "smallint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_transactiontypes", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "transactions",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    createdon = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    cnabfileid = table.Column<Guid>(type: "uuid", nullable: true),
                    transactiontypeid1 = table.Column<int>(type: "integer", nullable: false),
                    transactiondatetime = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    amountcnab = table.Column<decimal>(type: "numeric", nullable: false),
                    beneficiary_document = table.Column<string>(type: "varchar", maxLength: 11, nullable: false),
                    card_number = table.Column<string>(type: "varchar", maxLength: 12, nullable: false),
                    store_name = table.Column<string>(type: "varchar", maxLength: 19, nullable: false),
                    store_owner = table.Column<string>(type: "varchar", maxLength: 14, nullable: false),
                    transactiontypeid = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_transactions", x => x.id);
                    table.ForeignKey(
                        name: "fk_transactions_transactiontypes_transactiontypeid1",
                        column: x => x.transactiontypeid1,
                        principalTable: "transactiontypes",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "transactiontypes",
                columns: new[] { "id", "description", "nature" },
                values: new object[,]
                {
                    { 1, "Debit", (byte)1 },
                    { 2, "Bank Slip", (byte)2 },
                    { 3, "Funding", (byte)2 },
                    { 4, "Credit", (byte)1 },
                    { 5, "Loan Receipt", (byte)1 },
                    { 6, "Sales", (byte)1 },
                    { 7, "TED Receipt", (byte)1 },
                    { 8, "DOC Receipt", (byte)1 },
                    { 9, "Rent", (byte)2 }
                });

            migrationBuilder.CreateIndex(
                name: "ix_cnabfiles_failedon",
                table: "cnabfiles",
                column: "failedon");

            migrationBuilder.CreateIndex(
                name: "ix_cnabfiles_filepath",
                table: "cnabfiles",
                column: "filepath",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_cnabfiles_processedon",
                table: "cnabfiles",
                column: "processedon");

            migrationBuilder.CreateIndex(
                name: "ix_cnabfiles_uploadedon",
                table: "cnabfiles",
                column: "uploadedon");

            migrationBuilder.CreateIndex(
                name: "ix_transactions_cnabfileid",
                table: "transactions",
                column: "cnabfileid");

            migrationBuilder.CreateIndex(
                name: "ix_transactions_store_name",
                table: "transactions",
                column: "store_name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_transactions_transactiondatetime",
                table: "transactions",
                column: "transactiondatetime",
                descending: new bool[0]);

            migrationBuilder.CreateIndex(
                name: "ix_transactions_transactiontypeid1",
                table: "transactions",
                column: "transactiontypeid1");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "cnabfiles");

            migrationBuilder.DropTable(
                name: "transactions");

            migrationBuilder.DropTable(
                name: "transactiontypes");
        }
    }
}
