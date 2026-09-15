using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TatumConnectBackened.Migrations
{
    /// <inheritdoc />
    public partial class ConvertProductCategoryToStringAndFixRelationships : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // NOTE: These FK names reflect the tables as actually created by the legacy
            // "AddTransactionEntity" migration (Data/Migrations), which pointed Product/Transactions
            // at an orphaned singular "Biller" table instead of the real "Billers" table.
            migrationBuilder.DropForeignKey(
                name: "FK_Product_Biller_BillerId",
                table: "Product");

            migrationBuilder.DropForeignKey(
                name: "FK_ProductItem_Product_ProductId",
                table: "ProductItem");

            migrationBuilder.DropForeignKey(
                name: "FK_Transactions_ProductItem_ProductItemId",
                table: "Transactions");

            migrationBuilder.DropForeignKey(
                name: "FK_Transactions_Product_ProductId",
                table: "Transactions");

            migrationBuilder.DropForeignKey(
                name: "FK_Transactions_Biller_BillerId",
                table: "Transactions");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ProductItem",
                table: "ProductItem");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Product",
                table: "Product");

            migrationBuilder.RenameTable(
                name: "ProductItem",
                newName: "ProductItems");

            migrationBuilder.RenameTable(
                name: "Product",
                newName: "Products");

            migrationBuilder.RenameIndex(
                name: "IX_ProductItem_ProductId",
                table: "ProductItems",
                newName: "IX_ProductItems_ProductId");

            migrationBuilder.RenameIndex(
                name: "IX_Product_BillerId",
                table: "Products",
                newName: "IX_Products_BillerId");

            migrationBuilder.AlterColumn<string>(
                name: "Code",
                table: "Products",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Category",
                table: "Products",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ProductItems",
                table: "ProductItems",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Products",
                table: "Products",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_Products_Code",
                table: "Products",
                column: "Code",
                unique: true);

            // Drop the orphaned singular "Biller" table left behind by the legacy
            // "AddTransactionEntity" migration. It duplicates "Billers" and is never
            // used by the application (empty in every environment).
            migrationBuilder.DropTable(
                name: "Biller");

            migrationBuilder.AddForeignKey(
                name: "FK_ProductItems_Products_ProductId",
                table: "ProductItems",
                column: "ProductId",
                principalTable: "Products",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Products_Billers_BillerId",
                table: "Products",
                column: "BillerId",
                principalTable: "Billers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Transactions_ProductItems_ProductItemId",
                table: "Transactions",
                column: "ProductItemId",
                principalTable: "ProductItems",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Transactions_Products_ProductId",
                table: "Transactions",
                column: "ProductId",
                principalTable: "Products",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Transactions_Billers_BillerId",
                table: "Transactions",
                column: "BillerId",
                principalTable: "Billers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProductItems_Products_ProductId",
                table: "ProductItems");

            migrationBuilder.DropForeignKey(
                name: "FK_Products_Billers_BillerId",
                table: "Products");

            migrationBuilder.DropForeignKey(
                name: "FK_Transactions_ProductItems_ProductItemId",
                table: "Transactions");

            migrationBuilder.DropForeignKey(
                name: "FK_Transactions_Products_ProductId",
                table: "Transactions");

            migrationBuilder.DropForeignKey(
                name: "FK_Transactions_Billers_BillerId",
                table: "Transactions");

            migrationBuilder.CreateTable(
                name: "Biller",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Code = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Category = table.Column<int>(type: "int", nullable: false),
                    LogoUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Biller", x => x.Id);
                });

            migrationBuilder.DropPrimaryKey(
                name: "PK_Products",
                table: "Products");

            migrationBuilder.DropIndex(
                name: "IX_Products_Code",
                table: "Products");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ProductItems",
                table: "ProductItems");

            migrationBuilder.RenameTable(
                name: "Products",
                newName: "Product");

            migrationBuilder.RenameTable(
                name: "ProductItems",
                newName: "ProductItem");

            migrationBuilder.RenameIndex(
                name: "IX_Products_BillerId",
                table: "Product",
                newName: "IX_Product_BillerId");

            migrationBuilder.RenameIndex(
                name: "IX_ProductItems_ProductId",
                table: "ProductItem",
                newName: "IX_ProductItem_ProductId");

            migrationBuilder.AlterColumn<string>(
                name: "Code",
                table: "Product",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AlterColumn<int>(
                name: "Category",
                table: "Product",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Product",
                table: "Product",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ProductItem",
                table: "ProductItem",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Product_Biller_BillerId",
                table: "Product",
                column: "BillerId",
                principalTable: "Biller",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ProductItem_Product_ProductId",
                table: "ProductItem",
                column: "ProductId",
                principalTable: "Product",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Transactions_ProductItem_ProductItemId",
                table: "Transactions",
                column: "ProductItemId",
                principalTable: "ProductItem",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Transactions_Product_ProductId",
                table: "Transactions",
                column: "ProductId",
                principalTable: "Product",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Transactions_Biller_BillerId",
                table: "Transactions",
                column: "BillerId",
                principalTable: "Biller",
                principalColumn: "Id");
        }
    }
}
