using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PersonalFinanceManagement.Migrations
{
    /// <inheritdoc />
    public partial class AddCategories : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CatCode",
                table: "transactions",
                type: "character varying(64)",
                maxLength: 64,
                nullable: true);

            migrationBuilder.CreateTable(
                name: "categories",
                columns: table => new
                {
                    Code = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    ParentCode = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
                    Name = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_categories", x => x.Code);
                    table.ForeignKey(
                        name: "FK_categories_categories_ParentCode",
                        column: x => x.ParentCode,
                        principalTable: "categories",
                        principalColumn: "Code");
                });

            migrationBuilder.CreateIndex(
                name: "IX_transactions_CatCode",
                table: "transactions",
                column: "CatCode");

            migrationBuilder.CreateIndex(
                name: "IX_categories_ParentCode",
                table: "categories",
                column: "ParentCode");

            migrationBuilder.AddForeignKey(
                name: "FK_transactions_categories_CatCode",
                table: "transactions",
                column: "CatCode",
                principalTable: "categories",
                principalColumn: "Code");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_transactions_categories_CatCode",
                table: "transactions");

            migrationBuilder.DropTable(
                name: "categories");

            migrationBuilder.DropIndex(
                name: "IX_transactions_CatCode",
                table: "transactions");

            migrationBuilder.DropColumn(
                name: "CatCode",
                table: "transactions");
        }
    }
}
