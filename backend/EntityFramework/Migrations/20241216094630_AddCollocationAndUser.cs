using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EntityFramework.Migrations
{
    /// <inheritdoc />
    public partial class AddCollocationAndUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "CollocationId",
                table: "Reminder",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "CollocationId",
                table: "Chore",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateTable(
                name: "Collocation",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Address = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Collocation", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "User",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    LastConnection = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CollocationId = table.Column<Guid>(type: "uuid", nullable: false),
                    Username = table.Column<string>(type: "text", nullable: false),
                    Email = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_User", x => x.Id);
                    table.ForeignKey(
                        name: "FK_User_Collocation_CollocationId",
                        column: x => x.CollocationId,
                        principalTable: "Collocation",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Reminder_CollocationId",
                table: "Reminder",
                column: "CollocationId");

            migrationBuilder.CreateIndex(
                name: "IX_Chore_CollocationId",
                table: "Chore",
                column: "CollocationId");

            migrationBuilder.CreateIndex(
                name: "IX_User_CollocationId",
                table: "User",
                column: "CollocationId");

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
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Chore_Collocation_CollocationId",
                table: "Chore");

            migrationBuilder.DropForeignKey(
                name: "FK_Reminder_Collocation_CollocationId",
                table: "Reminder");

            migrationBuilder.DropTable(
                name: "User");

            migrationBuilder.DropTable(
                name: "Collocation");

            migrationBuilder.DropIndex(
                name: "IX_Reminder_CollocationId",
                table: "Reminder");

            migrationBuilder.DropIndex(
                name: "IX_Chore_CollocationId",
                table: "Chore");

            migrationBuilder.DropColumn(
                name: "CollocationId",
                table: "Reminder");

            migrationBuilder.DropColumn(
                name: "CollocationId",
                table: "Chore");
        }
    }
}
