using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ClassLibrary1.Migrations
{
    /// <inheritdoc />
    public partial class Guifj3d : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_IdempotencyId_HttpExchangeData_HttpExchangeDataId",
                table: "IdempotencyId");

            migrationBuilder.DropIndex(
                name: "IX_IdempotencyId_HttpExchangeDataId",
                table: "IdempotencyId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_IdempotencyId_HttpExchangeDataId",
                table: "IdempotencyId",
                column: "HttpExchangeDataId");

            migrationBuilder.AddForeignKey(
                name: "FK_IdempotencyId_HttpExchangeData_HttpExchangeDataId",
                table: "IdempotencyId",
                column: "HttpExchangeDataId",
                principalTable: "HttpExchangeData",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
