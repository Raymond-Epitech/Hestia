using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EntityFramework.Migrations
{
    /// <inheritdoc />
    public partial class RenameCollocationToColocation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Chore_Collocation_CollocationId",
                table: "Chore");

            migrationBuilder.DropForeignKey(
                name: "FK_Reminder_Collocation_CollocationId",
                table: "Reminder");

            migrationBuilder.DropForeignKey(
                name: "FK_User_Collocation_CollocationId",
                table: "User");

            migrationBuilder.DropTable(
                name: "Collocation");

            migrationBuilder.RenameColumn(
                name: "CollocationId",
                table: "User",
                newName: "ColocationId");

            migrationBuilder.RenameIndex(
                name: "IX_User_CollocationId",
                table: "User",
                newName: "IX_User_ColocationId");

            migrationBuilder.RenameColumn(
                name: "CollocationId",
                table: "Reminder",
                newName: "ColocationId");

            migrationBuilder.RenameIndex(
                name: "IX_Reminder_CollocationId",
                table: "Reminder",
                newName: "IX_Reminder_ColocationId");

            migrationBuilder.RenameColumn(
                name: "CollocationId",
                table: "Chore",
                newName: "ColocationId");

            migrationBuilder.RenameIndex(
                name: "IX_Chore_CollocationId",
                table: "Chore",
                newName: "IX_Chore_ColocationId");

            migrationBuilder.CreateTable(
                name: "Colocation",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Address = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Colocation", x => x.Id);
                });

            migrationBuilder.AddForeignKey(
                name: "FK_Chore_Colocation_ColocationId",
                table: "Chore",
                column: "ColocationId",
                principalTable: "Colocation",
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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Chore_Colocation_ColocationId",
                table: "Chore");

            migrationBuilder.DropForeignKey(
                name: "FK_Reminder_Colocation_ColocationId",
                table: "Reminder");

            migrationBuilder.DropForeignKey(
                name: "FK_User_Colocation_ColocationId",
                table: "User");

            migrationBuilder.DropTable(
                name: "Colocation");

            migrationBuilder.RenameColumn(
                name: "ColocationId",
                table: "User",
                newName: "CollocationId");

            migrationBuilder.RenameIndex(
                name: "IX_User_ColocationId",
                table: "User",
                newName: "IX_User_CollocationId");

            migrationBuilder.RenameColumn(
                name: "ColocationId",
                table: "Reminder",
                newName: "CollocationId");

            migrationBuilder.RenameIndex(
                name: "IX_Reminder_ColocationId",
                table: "Reminder",
                newName: "IX_Reminder_CollocationId");

            migrationBuilder.RenameColumn(
                name: "ColocationId",
                table: "Chore",
                newName: "CollocationId");

            migrationBuilder.RenameIndex(
                name: "IX_Chore_ColocationId",
                table: "Chore",
                newName: "IX_Chore_CollocationId");

            migrationBuilder.CreateTable(
                name: "Collocation",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Address = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Collocation", x => x.Id);
                });

            migrationBuilder.AddForeignKey(
                name: "FK_Chore_Collocation_CollocationId",
                table: "Chore",
                column: "CollocationId",
                principalTable: "Collocation",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Reminder_Collocation_CollocationId",
                table: "Reminder",
                column: "CollocationId",
                principalTable: "Collocation",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_User_Collocation_CollocationId",
                table: "User",
                column: "CollocationId",
                principalTable: "Collocation",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }
    }
}
