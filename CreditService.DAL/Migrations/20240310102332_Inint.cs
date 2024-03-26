using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ClassLibrary1.Migrations
{
    /// <inheritdoc />
    public partial class Inint : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CreditRules",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    AmountMax_Amount = table.Column<decimal>(type: "numeric", nullable: false),
                    AmountMax_Currency = table.Column<int>(type: "integer", nullable: false),
                    AmountMin_Amount = table.Column<decimal>(type: "numeric", nullable: false),
                    AmountMin_Currency = table.Column<int>(type: "integer", nullable: false),
                    InterestRateMax = table.Column<double>(type: "double precision", nullable: false),
                    InterestRateMin = table.Column<double>(type: "double precision", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Term = table.Column<int>(type: "integer", nullable: false),
                    LoanIds = table.Column<List<Guid>>(type: "uuid[]", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CreditRules", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Loan",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Amount = table.Column<decimal>(type: "numeric", nullable: false),
                    InterestRate = table.Column<double>(type: "double precision", nullable: false),
                    CurrencyType = table.Column<int>(type: "integer", nullable: false),
                    CreditRulesId = table.Column<Guid>(type: "uuid", nullable: false),
                    LoanAppId = table.Column<Guid>(type: "uuid", nullable: false),
                    AccountId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Loan", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "LoanApp",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    LoanStatus = table.Column<int>(type: "integer", nullable: false),
                    Date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: false),
                    LoanId = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LoanApp", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LoanApp_Loan_LoanId",
                        column: x => x.LoanId,
                        principalTable: "Loan",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_LoanApp_LoanId",
                table: "LoanApp",
                column: "LoanId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CreditRules");

            migrationBuilder.DropTable(
                name: "LoanApp");

            migrationBuilder.DropTable(
                name: "Loan");
        }
    }
}
