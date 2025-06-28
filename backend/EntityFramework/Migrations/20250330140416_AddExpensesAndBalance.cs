using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EntityFramework.Migrations
{
    /// <inheritdoc />
    public partial class AddExpensesAndBalance : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "CreatedBy",
                table: "Users",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.Sql("""
                ALTER TABLE "Reminders"
                ALTER COLUMN "CreatedBy" TYPE uuid
                USING "CreatedBy"::uuid;
            """);

            migrationBuilder.Sql("""
                ALTER TABLE "Expenses"
                ALTER COLUMN "CreatedBy" TYPE uuid
                USING "CreatedBy"::uuid;
            """);

            migrationBuilder.AddColumn<string>(
                name: "Category",
                table: "Expenses",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<Guid>(
                name: "ColocationId",
                table: "Entries",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "CreatedBy",
                table: "Entries",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.Sql("""
                ALTER TABLE "Colocations"
                ALTER COLUMN "CreatedBy" TYPE uuid
                USING "CreatedBy"::uuid;
            """);

            migrationBuilder.Sql("""
                ALTER TABLE "Chores"
                ALTER COLUMN "CreatedBy" TYPE uuid
                USING "CreatedBy"::uuid;
            """);

            migrationBuilder.Sql("""
                ALTER TABLE "ChoreMessages"
                ALTER COLUMN "CreatedBy" TYPE uuid
                USING "CreatedBy"::uuid;
            """);

            migrationBuilder.CreateTable(
                name: "ShoppingList",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    ColocationId = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ShoppingList", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ShoppingList_Colocations_ColocationId",
                        column: x => x.ColocationId,
                        principalTable: "Colocations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Entries_ColocationId",
                table: "Entries",
                column: "ColocationId");

            migrationBuilder.CreateIndex(
                name: "IX_ShoppingList_ColocationId",
                table: "ShoppingList",
                column: "ColocationId");

            migrationBuilder.AddForeignKey(
                name: "FK_Entries_Colocations_ColocationId",
                table: "Entries",
                column: "ColocationId",
                principalTable: "Colocations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Entries_Colocations_ColocationId",
                table: "Entries");

            migrationBuilder.DropTable(
                name: "ShoppingList");

            migrationBuilder.DropIndex(
                name: "IX_Entries_ColocationId",
                table: "Entries");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "Category",
                table: "Expenses");

            migrationBuilder.DropColumn(
                name: "ColocationId",
                table: "Entries");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "Entries");

            migrationBuilder.Sql("""
                ALTER TABLE "Reminders"
                ALTER COLUMN "CreatedBy" TYPE text
                USING "CreatedBy"::text;
            """);

            migrationBuilder.Sql("""
                ALTER TABLE "Expenses"
                ALTER COLUMN "CreatedBy" TYPE text
                USING "CreatedBy"::text;
            """);

            migrationBuilder.Sql("""
                ALTER TABLE "Colocations"
                ALTER COLUMN "CreatedBy" TYPE text
                USING "CreatedBy"::text;
            """);

            migrationBuilder.Sql("""
                ALTER TABLE "Chores"
                ALTER COLUMN "CreatedBy" TYPE text
                USING "CreatedBy"::text;
            """);

            migrationBuilder.Sql("""
                ALTER TABLE "ChoreMessages"
                ALTER COLUMN "CreatedBy" TYPE text
                USING "CreatedBy"::text;
            """);
        }
    }
}
