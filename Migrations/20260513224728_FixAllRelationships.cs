using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace dance.API.Migrations
{
    /// <inheritdoc />
    public partial class FixAllRelationships : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Groups_Directions_Direction_id",
                table: "Groups");

            migrationBuilder.DropForeignKey(
                name: "FK_Groups_Trainers_Trainer_id",
                table: "Groups");

            migrationBuilder.DropForeignKey(
                name: "FK_Registrations_Clients_Client_id1",
                table: "Registrations");

            migrationBuilder.DropForeignKey(
                name: "FK_Registrations_Groups_Group_id1",
                table: "Registrations");

            migrationBuilder.DropForeignKey(
                name: "FK_Subscriptions_Clients_Client_id1",
                table: "Subscriptions");

            migrationBuilder.DropForeignKey(
                name: "FK_Subscriptions_Groups_Group_id1",
                table: "Subscriptions");

            migrationBuilder.DropForeignKey(
                name: "FK_Users_Clients_Client_id1",
                table: "Users");

            migrationBuilder.DropForeignKey(
                name: "FK_Users_Trainers_Trainer_id1",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_Users_Client_id1",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_Users_Trainer_id1",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_Subscriptions_Client_id1",
                table: "Subscriptions");

            migrationBuilder.DropIndex(
                name: "IX_Subscriptions_Group_id1",
                table: "Subscriptions");

            migrationBuilder.DropIndex(
                name: "IX_Registrations_Client_id1",
                table: "Registrations");

            migrationBuilder.DropIndex(
                name: "IX_Registrations_Group_id1",
                table: "Registrations");

            migrationBuilder.DropColumn(
                name: "Client_id1",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "Trainer_id1",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "Client_id1",
                table: "Subscriptions");

            migrationBuilder.DropColumn(
                name: "Group_id1",
                table: "Subscriptions");

            migrationBuilder.DropColumn(
                name: "Client_id1",
                table: "Registrations");

            migrationBuilder.DropColumn(
                name: "Group_id1",
                table: "Registrations");

            migrationBuilder.AddColumn<int>(
                name: "User_id",
                table: "Trainers",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "Subscriptions",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "User_id",
                table: "Clients",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Trainers_User_id",
                table: "Trainers",
                column: "User_id",
                unique: true,
                filter: "[User_id] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Subscriptions_Client_id",
                table: "Subscriptions",
                column: "Client_id");

            migrationBuilder.CreateIndex(
                name: "IX_Subscriptions_Group_id",
                table: "Subscriptions",
                column: "Group_id");

            migrationBuilder.CreateIndex(
                name: "IX_Registrations_Client_id",
                table: "Registrations",
                column: "Client_id");

            migrationBuilder.CreateIndex(
                name: "IX_Registrations_Group_id",
                table: "Registrations",
                column: "Group_id");

            migrationBuilder.CreateIndex(
                name: "IX_Clients_User_id",
                table: "Clients",
                column: "User_id",
                unique: true,
                filter: "[User_id] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_Clients_Users_User_id",
                table: "Clients",
                column: "User_id",
                principalTable: "Users",
                principalColumn: "User_id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Groups_Directions_Direction_id",
                table: "Groups",
                column: "Direction_id",
                principalTable: "Directions",
                principalColumn: "Direction_id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Groups_Trainers_Trainer_id",
                table: "Groups",
                column: "Trainer_id",
                principalTable: "Trainers",
                principalColumn: "Trainer_id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Registrations_Clients_Client_id",
                table: "Registrations",
                column: "Client_id",
                principalTable: "Clients",
                principalColumn: "Client_id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Registrations_Groups_Group_id",
                table: "Registrations",
                column: "Group_id",
                principalTable: "Groups",
                principalColumn: "Group_id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Subscriptions_Clients_Client_id",
                table: "Subscriptions",
                column: "Client_id",
                principalTable: "Clients",
                principalColumn: "Client_id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Subscriptions_Groups_Group_id",
                table: "Subscriptions",
                column: "Group_id",
                principalTable: "Groups",
                principalColumn: "Group_id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Trainers_Users_User_id",
                table: "Trainers",
                column: "User_id",
                principalTable: "Users",
                principalColumn: "User_id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Clients_Users_User_id",
                table: "Clients");

            migrationBuilder.DropForeignKey(
                name: "FK_Groups_Directions_Direction_id",
                table: "Groups");

            migrationBuilder.DropForeignKey(
                name: "FK_Groups_Trainers_Trainer_id",
                table: "Groups");

            migrationBuilder.DropForeignKey(
                name: "FK_Registrations_Clients_Client_id",
                table: "Registrations");

            migrationBuilder.DropForeignKey(
                name: "FK_Registrations_Groups_Group_id",
                table: "Registrations");

            migrationBuilder.DropForeignKey(
                name: "FK_Subscriptions_Clients_Client_id",
                table: "Subscriptions");

            migrationBuilder.DropForeignKey(
                name: "FK_Subscriptions_Groups_Group_id",
                table: "Subscriptions");

            migrationBuilder.DropForeignKey(
                name: "FK_Trainers_Users_User_id",
                table: "Trainers");

            migrationBuilder.DropIndex(
                name: "IX_Trainers_User_id",
                table: "Trainers");

            migrationBuilder.DropIndex(
                name: "IX_Subscriptions_Client_id",
                table: "Subscriptions");

            migrationBuilder.DropIndex(
                name: "IX_Subscriptions_Group_id",
                table: "Subscriptions");

            migrationBuilder.DropIndex(
                name: "IX_Registrations_Client_id",
                table: "Registrations");

            migrationBuilder.DropIndex(
                name: "IX_Registrations_Group_id",
                table: "Registrations");

            migrationBuilder.DropIndex(
                name: "IX_Clients_User_id",
                table: "Clients");

            migrationBuilder.DropColumn(
                name: "User_id",
                table: "Trainers");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "Subscriptions");

            migrationBuilder.DropColumn(
                name: "User_id",
                table: "Clients");

            migrationBuilder.AddColumn<int>(
                name: "Client_id1",
                table: "Users",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Trainer_id1",
                table: "Users",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Client_id1",
                table: "Subscriptions",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Group_id1",
                table: "Subscriptions",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Client_id1",
                table: "Registrations",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Group_id1",
                table: "Registrations",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_Client_id1",
                table: "Users",
                column: "Client_id1");

            migrationBuilder.CreateIndex(
                name: "IX_Users_Trainer_id1",
                table: "Users",
                column: "Trainer_id1");

            migrationBuilder.CreateIndex(
                name: "IX_Subscriptions_Client_id1",
                table: "Subscriptions",
                column: "Client_id1");

            migrationBuilder.CreateIndex(
                name: "IX_Subscriptions_Group_id1",
                table: "Subscriptions",
                column: "Group_id1");

            migrationBuilder.CreateIndex(
                name: "IX_Registrations_Client_id1",
                table: "Registrations",
                column: "Client_id1");

            migrationBuilder.CreateIndex(
                name: "IX_Registrations_Group_id1",
                table: "Registrations",
                column: "Group_id1");

            migrationBuilder.AddForeignKey(
                name: "FK_Groups_Directions_Direction_id",
                table: "Groups",
                column: "Direction_id",
                principalTable: "Directions",
                principalColumn: "Direction_id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Groups_Trainers_Trainer_id",
                table: "Groups",
                column: "Trainer_id",
                principalTable: "Trainers",
                principalColumn: "Trainer_id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Registrations_Clients_Client_id1",
                table: "Registrations",
                column: "Client_id1",
                principalTable: "Clients",
                principalColumn: "Client_id");

            migrationBuilder.AddForeignKey(
                name: "FK_Registrations_Groups_Group_id1",
                table: "Registrations",
                column: "Group_id1",
                principalTable: "Groups",
                principalColumn: "Group_id");

            migrationBuilder.AddForeignKey(
                name: "FK_Subscriptions_Clients_Client_id1",
                table: "Subscriptions",
                column: "Client_id1",
                principalTable: "Clients",
                principalColumn: "Client_id");

            migrationBuilder.AddForeignKey(
                name: "FK_Subscriptions_Groups_Group_id1",
                table: "Subscriptions",
                column: "Group_id1",
                principalTable: "Groups",
                principalColumn: "Group_id");

            migrationBuilder.AddForeignKey(
                name: "FK_Users_Clients_Client_id1",
                table: "Users",
                column: "Client_id1",
                principalTable: "Clients",
                principalColumn: "Client_id");

            migrationBuilder.AddForeignKey(
                name: "FK_Users_Trainers_Trainer_id1",
                table: "Users",
                column: "Trainer_id1",
                principalTable: "Trainers",
                principalColumn: "Trainer_id");
        }
    }
}
