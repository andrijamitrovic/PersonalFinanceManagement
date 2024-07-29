using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PersonalFinanceManagement.Migrations
{
    /// <inheritdoc />
    public partial class AddSplitTransaction : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "SplitId",
                table: "transactions",
                type: "character varying(64)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_transactions_SplitId",
                table: "transactions",
                column: "SplitId");

            migrationBuilder.AddForeignKey(
                name: "FK_transactions_transactions_SplitId",
                table: "transactions",
                column: "SplitId",
                principalTable: "transactions",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_transactions_transactions_SplitId",
                table: "transactions");

            migrationBuilder.DropIndex(
                name: "IX_transactions_SplitId",
                table: "transactions");

            migrationBuilder.DropColumn(
                name: "SplitId",
                table: "transactions");
        }
    }
}
