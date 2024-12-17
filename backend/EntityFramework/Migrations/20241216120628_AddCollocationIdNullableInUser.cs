using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EntityFramework.Migrations
{
    /// <inheritdoc />
    public partial class AddCollocationIdNullableInUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_User_Collocation_CollocationId",
                table: "User");

            migrationBuilder.AlterColumn<Guid>(
                name: "CollocationId",
                table: "User",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "Collocation",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                table: "Collocation",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddForeignKey(
                name: "FK_User_Collocation_CollocationId",
                table: "User",
                column: "CollocationId",
                principalTable: "Collocation",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_User_Collocation_CollocationId",
                table: "User");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "Collocation");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "Collocation");

            migrationBuilder.AlterColumn<Guid>(
                name: "CollocationId",
                table: "User",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_User_Collocation_CollocationId",
                table: "User",
                column: "CollocationId",
                principalTable: "Collocation",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
