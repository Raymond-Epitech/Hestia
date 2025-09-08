using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EntityFramework.Migrations
{
    /// <inheritdoc />
    public partial class ReminderRefacto : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Reminders_Users_UserId",
                table: "Reminders");

            migrationBuilder.DropForeignKey(
                name: "FK_ShoppingItems_ShoppingList_ShoppingListId",
                table: "ShoppingItems");

            migrationBuilder.DropTable(
                name: "ShoppingList");

            migrationBuilder.DropIndex(
                name: "IX_Reminders_UserId",
                table: "Reminders");

            migrationBuilder.DropColumn(
                name: "IsImage",
                table: "Reminders");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Reminders");

            migrationBuilder.RenameColumn(
                name: "ShoppingListId",
                table: "ShoppingItems",
                newName: "ShoppingListReminderId");

            migrationBuilder.RenameIndex(
                name: "IX_ShoppingItems_ShoppingListId",
                table: "ShoppingItems",
                newName: "IX_ShoppingItems_ShoppingListReminderId");

            migrationBuilder.AddColumn<string>(
                name: "Language",
                table: "Users",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<string>(
                name: "Content",
                table: "Reminders",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "Color",
                table: "Reminders",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AddColumn<bool>(
                name: "AllowMultipleChoices",
                table: "Reminders",
                type: "boolean",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "Reminders",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ExpirationDate",
                table: "Reminders",
                type: "timestamp without time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ImageUrl",
                table: "Reminders",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsAnonymous",
                table: "Reminders",
                type: "boolean",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ReminderType",
                table: "Reminders",
                type: "character varying(13)",
                maxLength: 13,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ShoppingListName",
                table: "Reminders",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Title",
                table: "Reminders",
                type: "text",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "PollVotes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    PollReminderId = table.Column<Guid>(type: "uuid", nullable: false),
                    VotedBy = table.Column<Guid>(type: "uuid", nullable: false),
                    VotedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    Choice = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PollVotes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PollVotes_Reminders_PollReminderId",
                        column: x => x.PollReminderId,
                        principalTable: "Reminders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PollVotes_Users_VotedBy",
                        column: x => x.VotedBy,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Reactions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    ReminderId = table.Column<Guid>(type: "uuid", nullable: false),
                    Type = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Reactions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Reactions_Reminders_ReminderId",
                        column: x => x.ReminderId,
                        principalTable: "Reminders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Reactions_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Reminders_CreatedBy",
                table: "Reminders",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_Messages_SentBy",
                table: "Messages",
                column: "SentBy");

            migrationBuilder.CreateIndex(
                name: "IX_Chores_CreatedBy",
                table: "Chores",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_ChoreMessages_CreatedBy",
                table: "ChoreMessages",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_PollVotes_PollReminderId",
                table: "PollVotes",
                column: "PollReminderId");

            migrationBuilder.CreateIndex(
                name: "IX_PollVotes_VotedBy",
                table: "PollVotes",
                column: "VotedBy");

            migrationBuilder.CreateIndex(
                name: "IX_Reactions_ReminderId",
                table: "Reactions",
                column: "ReminderId");

            migrationBuilder.CreateIndex(
                name: "IX_Reactions_UserId",
                table: "Reactions",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_ChoreMessages_Users_CreatedBy",
                table: "ChoreMessages",
                column: "CreatedBy",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Chores_Users_CreatedBy",
                table: "Chores",
                column: "CreatedBy",
                principalTable: "Users",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Messages_Users_SentBy",
                table: "Messages",
                column: "SentBy",
                principalTable: "Users",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Reminders_Users_CreatedBy",
                table: "Reminders",
                column: "CreatedBy",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ShoppingItems_Reminders_ShoppingListReminderId",
                table: "ShoppingItems",
                column: "ShoppingListReminderId",
                principalTable: "Reminders",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ChoreMessages_Users_CreatedBy",
                table: "ChoreMessages");

            migrationBuilder.DropForeignKey(
                name: "FK_Chores_Users_CreatedBy",
                table: "Chores");

            migrationBuilder.DropForeignKey(
                name: "FK_Messages_Users_SentBy",
                table: "Messages");

            migrationBuilder.DropForeignKey(
                name: "FK_Reminders_Users_CreatedBy",
                table: "Reminders");

            migrationBuilder.DropForeignKey(
                name: "FK_ShoppingItems_Reminders_ShoppingListReminderId",
                table: "ShoppingItems");

            migrationBuilder.DropTable(
                name: "PollVotes");

            migrationBuilder.DropTable(
                name: "Reactions");

            migrationBuilder.DropIndex(
                name: "IX_Reminders_CreatedBy",
                table: "Reminders");

            migrationBuilder.DropIndex(
                name: "IX_Messages_SentBy",
                table: "Messages");

            migrationBuilder.DropIndex(
                name: "IX_Chores_CreatedBy",
                table: "Chores");

            migrationBuilder.DropIndex(
                name: "IX_ChoreMessages_CreatedBy",
                table: "ChoreMessages");

            migrationBuilder.DropColumn(
                name: "Language",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "AllowMultipleChoices",
                table: "Reminders");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "Reminders");

            migrationBuilder.DropColumn(
                name: "ExpirationDate",
                table: "Reminders");

            migrationBuilder.DropColumn(
                name: "ImageUrl",
                table: "Reminders");

            migrationBuilder.DropColumn(
                name: "IsAnonymous",
                table: "Reminders");

            migrationBuilder.DropColumn(
                name: "ReminderType",
                table: "Reminders");

            migrationBuilder.DropColumn(
                name: "ShoppingListName",
                table: "Reminders");

            migrationBuilder.DropColumn(
                name: "Title",
                table: "Reminders");

            migrationBuilder.RenameColumn(
                name: "ShoppingListReminderId",
                table: "ShoppingItems",
                newName: "ShoppingListId");

            migrationBuilder.RenameIndex(
                name: "IX_ShoppingItems_ShoppingListReminderId",
                table: "ShoppingItems",
                newName: "IX_ShoppingItems_ShoppingListId");

            migrationBuilder.AlterColumn<string>(
                name: "Content",
                table: "Reminders",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Color",
                table: "Reminders",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsImage",
                table: "Reminders",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<Guid>(
                name: "UserId",
                table: "Reminders",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "ShoppingList",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ColocationId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: false),
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
                name: "IX_Reminders_UserId",
                table: "Reminders",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_ShoppingList_ColocationId",
                table: "ShoppingList",
                column: "ColocationId");

            migrationBuilder.AddForeignKey(
                name: "FK_Reminders_Users_UserId",
                table: "Reminders",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ShoppingItems_ShoppingList_ShoppingListId",
                table: "ShoppingItems",
                column: "ShoppingListId",
                principalTable: "ShoppingList",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
