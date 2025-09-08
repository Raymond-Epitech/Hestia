using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EntityFramework.Migrations
{
    /// <inheritdoc />
    public partial class FCMTokenManyToMany : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FCMDevices_Users_UserId",
                table: "FCMDevices");

            migrationBuilder.DropIndex(
                name: "IX_FCMDevices_UserId",
                table: "FCMDevices");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "FCMDevices");

            migrationBuilder.AddColumn<Guid>(
                name: "UserId",
                table: "Reminders",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "UserFCMDevices",
                columns: table => new
                {
                    FCMDevicesFCMToken = table.Column<string>(type: "text", nullable: false),
                    UsersId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserFCMDevices", x => new { x.FCMDevicesFCMToken, x.UsersId });
                    table.ForeignKey(
                        name: "FK_UserFCMDevices_FCMDevices_FCMDevicesFCMToken",
                        column: x => x.FCMDevicesFCMToken,
                        principalTable: "FCMDevices",
                        principalColumn: "FCMToken",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserFCMDevices_Users_UsersId",
                        column: x => x.UsersId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Reminders_UserId",
                table: "Reminders",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserFCMDevices_UsersId",
                table: "UserFCMDevices",
                column: "UsersId");

            migrationBuilder.AddForeignKey(
                name: "FK_Reminders_Users_UserId",
                table: "Reminders",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Reminders_Users_UserId",
                table: "Reminders");

            migrationBuilder.DropTable(
                name: "UserFCMDevices");

            migrationBuilder.DropIndex(
                name: "IX_Reminders_UserId",
                table: "Reminders");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Reminders");

            migrationBuilder.AddColumn<Guid>(
                name: "UserId",
                table: "FCMDevices",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_FCMDevices_UserId",
                table: "FCMDevices",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_FCMDevices_Users_UserId",
                table: "FCMDevices",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
