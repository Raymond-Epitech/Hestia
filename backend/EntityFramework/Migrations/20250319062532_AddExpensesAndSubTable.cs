using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EntityFramework.Migrations
{
    /// <inheritdoc />
    public partial class AddExpensesAndSubTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Chore_Colocation_ColocationId",
                table: "Chore");

            migrationBuilder.DropForeignKey(
                name: "FK_ChoreEnrollments_Chore_ChoreId",
                table: "ChoreEnrollments");

            migrationBuilder.DropForeignKey(
                name: "FK_ChoreEnrollments_User_UserId",
                table: "ChoreEnrollments");

            migrationBuilder.DropForeignKey(
                name: "FK_ChoreMessage_Chore_ChoreId",
                table: "ChoreMessage");

            migrationBuilder.DropForeignKey(
                name: "FK_Reminder_Colocation_ColocationId",
                table: "Reminder");

            migrationBuilder.DropForeignKey(
                name: "FK_User_Colocation_ColocationId",
                table: "User");

            migrationBuilder.DropPrimaryKey(
                name: "PK_User",
                table: "User");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Reminder",
                table: "Reminder");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Colocation",
                table: "Colocation");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ChoreMessage",
                table: "ChoreMessage");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Chore",
                table: "Chore");

            migrationBuilder.RenameTable(
                name: "User",
                newName: "Users");

            migrationBuilder.RenameTable(
                name: "Reminder",
                newName: "Reminders");

            migrationBuilder.RenameTable(
                name: "Colocation",
                newName: "Colocations");

            migrationBuilder.RenameTable(
                name: "ChoreMessage",
                newName: "ChoreMessages");

            migrationBuilder.RenameTable(
                name: "Chore",
                newName: "Chores");

            migrationBuilder.RenameIndex(
                name: "IX_User_ColocationId",
                table: "Users",
                newName: "IX_Users_ColocationId");

            migrationBuilder.RenameIndex(
                name: "IX_Reminder_ColocationId",
                table: "Reminders",
                newName: "IX_Reminders_ColocationId");

            migrationBuilder.RenameIndex(
                name: "IX_ChoreMessage_ChoreId",
                table: "ChoreMessages",
                newName: "IX_ChoreMessages_ChoreId");

            migrationBuilder.RenameIndex(
                name: "IX_Chore_ColocationId",
                table: "Chores",
                newName: "IX_Chores_ColocationId");

            migrationBuilder.AlterColumn<Guid>(
                name: "ColocationId",
                table: "Users",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Users",
                table: "Users",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Reminders",
                table: "Reminders",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Colocations",
                table: "Colocations",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ChoreMessages",
                table: "ChoreMessages",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Chores",
                table: "Chores",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "Balances",
                columns: table => new
                {
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PersonalBalance = table.Column<decimal>(type: "decimal(19,2)", nullable: false),
                    LastUpdate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Balances", x => x.UserId);
                    table.ForeignKey(
                        name: "FK_Balances_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Expenses",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ColocationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Amount = table.Column<decimal>(type: "decimal(19,2)", nullable: false),
                    PaidBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SplitType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DateOfPayment = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Expenses", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Expenses_Colocations_ColocationId",
                        column: x => x.ColocationId,
                        principalTable: "Colocations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Expenses_Users_PaidBy",
                        column: x => x.PaidBy,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Entries",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ExpenseId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(19,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Entries", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Entries_Expenses_ExpenseId",
                        column: x => x.ExpenseId,
                        principalTable: "Expenses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Entries_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SplitBetweens",
                columns: table => new
                {
                    ExpenseId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SplitBetweens", x => new { x.UserId, x.ExpenseId });
                    table.ForeignKey(
                        name: "FK_SplitBetweens_Expenses_ExpenseId",
                        column: x => x.ExpenseId,
                        principalTable: "Expenses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SplitBetweens_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Entries_ExpenseId",
                table: "Entries",
                column: "ExpenseId");

            migrationBuilder.CreateIndex(
                name: "IX_Entries_UserId",
                table: "Entries",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Expenses_ColocationId",
                table: "Expenses",
                column: "ColocationId");

            migrationBuilder.CreateIndex(
                name: "IX_Expenses_PaidBy",
                table: "Expenses",
                column: "PaidBy");

            migrationBuilder.CreateIndex(
                name: "IX_SplitBetweens_ExpenseId",
                table: "SplitBetweens",
                column: "ExpenseId");

            migrationBuilder.AddForeignKey(
                name: "FK_ChoreEnrollments_Chores_ChoreId",
                table: "ChoreEnrollments",
                column: "ChoreId",
                principalTable: "Chores",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ChoreEnrollments_Users_UserId",
                table: "ChoreEnrollments",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ChoreMessages_Chores_ChoreId",
                table: "ChoreMessages",
                column: "ChoreId",
                principalTable: "Chores",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Chores_Colocations_ColocationId",
                table: "Chores",
                column: "ColocationId",
                principalTable: "Colocations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Reminders_Colocations_ColocationId",
                table: "Reminders",
                column: "ColocationId",
                principalTable: "Colocations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Users_Colocations_ColocationId",
                table: "Users",
                column: "ColocationId",
                principalTable: "Colocations",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ChoreEnrollments_Chores_ChoreId",
                table: "ChoreEnrollments");

            migrationBuilder.DropForeignKey(
                name: "FK_ChoreEnrollments_Users_UserId",
                table: "ChoreEnrollments");

            migrationBuilder.DropForeignKey(
                name: "FK_ChoreMessages_Chores_ChoreId",
                table: "ChoreMessages");

            migrationBuilder.DropForeignKey(
                name: "FK_Chores_Colocations_ColocationId",
                table: "Chores");

            migrationBuilder.DropForeignKey(
                name: "FK_Reminders_Colocations_ColocationId",
                table: "Reminders");

            migrationBuilder.DropForeignKey(
                name: "FK_Users_Colocations_ColocationId",
                table: "Users");

            migrationBuilder.DropTable(
                name: "Balances");

            migrationBuilder.DropTable(
                name: "Entries");

            migrationBuilder.DropTable(
                name: "SplitBetweens");

            migrationBuilder.DropTable(
                name: "Expenses");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Users",
                table: "Users");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Reminders",
                table: "Reminders");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Colocations",
                table: "Colocations");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Chores",
                table: "Chores");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ChoreMessages",
                table: "ChoreMessages");

            migrationBuilder.RenameTable(
                name: "Users",
                newName: "User");

            migrationBuilder.RenameTable(
                name: "Reminders",
                newName: "Reminder");

            migrationBuilder.RenameTable(
                name: "Colocations",
                newName: "Colocation");

            migrationBuilder.RenameTable(
                name: "Chores",
                newName: "Chore");

            migrationBuilder.RenameTable(
                name: "ChoreMessages",
                newName: "ChoreMessage");

            migrationBuilder.RenameIndex(
                name: "IX_Users_ColocationId",
                table: "User",
                newName: "IX_User_ColocationId");

            migrationBuilder.RenameIndex(
                name: "IX_Reminders_ColocationId",
                table: "Reminder",
                newName: "IX_Reminder_ColocationId");

            migrationBuilder.RenameIndex(
                name: "IX_Chores_ColocationId",
                table: "Chore",
                newName: "IX_Chore_ColocationId");

            migrationBuilder.RenameIndex(
                name: "IX_ChoreMessages_ChoreId",
                table: "ChoreMessage",
                newName: "IX_ChoreMessage_ChoreId");

            migrationBuilder.AlterColumn<Guid>(
                name: "ColocationId",
                table: "User",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AddPrimaryKey(
                name: "PK_User",
                table: "User",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Reminder",
                table: "Reminder",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Colocation",
                table: "Colocation",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Chore",
                table: "Chore",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ChoreMessage",
                table: "ChoreMessage",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Chore_Colocation_ColocationId",
                table: "Chore",
                column: "ColocationId",
                principalTable: "Colocation",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ChoreEnrollments_Chore_ChoreId",
                table: "ChoreEnrollments",
                column: "ChoreId",
                principalTable: "Chore",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ChoreEnrollments_User_UserId",
                table: "ChoreEnrollments",
                column: "UserId",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ChoreMessage_Chore_ChoreId",
                table: "ChoreMessage",
                column: "ChoreId",
                principalTable: "Chore",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Reminder_Colocation_ColocationId",
                table: "Reminder",
                column: "ColocationId",
                principalTable: "Colocation",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_User_Colocation_ColocationId",
                table: "User",
                column: "ColocationId",
                principalTable: "Colocation",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }
    }
}
