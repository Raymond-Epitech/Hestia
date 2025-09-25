using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EntityFramework.Migrations
{
    /// <inheritdoc />
    public partial class AddExpenseAutomations : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "ExpenseAutomationId",
                table: "Expenses",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "ExpenseAutomations",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    DayOfTheMonth = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExpenseAutomations", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Expenses_ExpenseAutomationId",
                table: "Expenses",
                column: "ExpenseAutomationId");

            migrationBuilder.AddForeignKey(
                name: "FK_Expenses_ExpenseAutomations_ExpenseAutomationId",
                table: "Expenses",
                column: "ExpenseAutomationId",
                principalTable: "ExpenseAutomations",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Expenses_ExpenseAutomations_ExpenseAutomationId",
                table: "Expenses");

            migrationBuilder.DropTable(
                name: "ExpenseAutomations");

            migrationBuilder.DropIndex(
                name: "IX_Expenses_ExpenseAutomationId",
                table: "Expenses");

            migrationBuilder.DropColumn(
                name: "ExpenseAutomationId",
                table: "Expenses");
        }
    }
}
