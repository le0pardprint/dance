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
            // Безопасное удаление foreign keys (могут не существовать)
            migrationBuilder.Sql(@"
                IF EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_Registrations_Clients_Client_id1')
                    ALTER TABLE [Registrations] DROP CONSTRAINT [FK_Registrations_Clients_Client_id1];
                IF EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_Registrations_Groups_Group_id1')
                    ALTER TABLE [Registrations] DROP CONSTRAINT [FK_Registrations_Groups_Group_id1];
                IF EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_Subscriptions_Clients_Client_id1')
                    ALTER TABLE [Subscriptions] DROP CONSTRAINT [FK_Subscriptions_Clients_Client_id1];
                IF EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_Subscriptions_Groups_Group_id1')
                    ALTER TABLE [Subscriptions] DROP CONSTRAINT [FK_Subscriptions_Groups_Group_id1];
                IF EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_Users_Clients_Client_id1')
                    ALTER TABLE [Users] DROP CONSTRAINT [FK_Users_Clients_Client_id1];
                IF EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_Users_Trainers_Trainer_id1')
                    ALTER TABLE [Users] DROP CONSTRAINT [FK_Users_Trainers_Trainer_id1];
            ");

            migrationBuilder.DropForeignKey(
                name: "FK_Groups_Directions_Direction_id",
                table: "Groups");

            migrationBuilder.DropForeignKey(
                name: "FK_Groups_Trainers_Trainer_id",
                table: "Groups");

            // Безопасное удаление индексов
            migrationBuilder.Sql(@"
                IF EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Users_Client_id1' AND object_id = OBJECT_ID('Users'))
                    DROP INDEX [IX_Users_Client_id1] ON [Users];
                IF EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Users_Trainer_id1' AND object_id = OBJECT_ID('Users'))
                    DROP INDEX [IX_Users_Trainer_id1] ON [Users];
                IF EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Subscriptions_Client_id1' AND object_id = OBJECT_ID('Subscriptions'))
                    DROP INDEX [IX_Subscriptions_Client_id1] ON [Subscriptions];
                IF EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Subscriptions_Group_id1' AND object_id = OBJECT_ID('Subscriptions'))
                    DROP INDEX [IX_Subscriptions_Group_id1] ON [Subscriptions];
                IF EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Registrations_Client_id1' AND object_id = OBJECT_ID('Registrations'))
                    DROP INDEX [IX_Registrations_Client_id1] ON [Registrations];
                IF EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Registrations_Group_id1' AND object_id = OBJECT_ID('Registrations'))
                    DROP INDEX [IX_Registrations_Group_id1] ON [Registrations];
            ");

            // Безопасное удаление колонок
            migrationBuilder.Sql(@"
                IF EXISTS (SELECT * FROM sys.columns WHERE name = 'Client_id1' AND object_id = OBJECT_ID('Users'))
                    ALTER TABLE [Users] DROP COLUMN [Client_id1];
                IF EXISTS (SELECT * FROM sys.columns WHERE name = 'Trainer_id1' AND object_id = OBJECT_ID('Users'))
                    ALTER TABLE [Users] DROP COLUMN [Trainer_id1];
                IF EXISTS (SELECT * FROM sys.columns WHERE name = 'Client_id1' AND object_id = OBJECT_ID('Subscriptions'))
                    ALTER TABLE [Subscriptions] DROP COLUMN [Client_id1];
                IF EXISTS (SELECT * FROM sys.columns WHERE name = 'Group_id1' AND object_id = OBJECT_ID('Subscriptions'))
                    ALTER TABLE [Subscriptions] DROP COLUMN [Group_id1];
                IF EXISTS (SELECT * FROM sys.columns WHERE name = 'Client_id1' AND object_id = OBJECT_ID('Registrations'))
                    ALTER TABLE [Registrations] DROP COLUMN [Client_id1];
                IF EXISTS (SELECT * FROM sys.columns WHERE name = 'Group_id1' AND object_id = OBJECT_ID('Registrations'))
                    ALTER TABLE [Registrations] DROP COLUMN [Group_id1];
            ");

            migrationBuilder.AddColumn<int>(
                name: "User_id",
                table: "Trainers",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "Subscriptions",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "User_id",
                table: "Clients",
                type: "int",
                nullable: true);

            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Trainers_User_id' AND object_id = OBJECT_ID('Trainers'))
                    CREATE UNIQUE INDEX [IX_Trainers_User_id] ON [Trainers] ([User_id]) WHERE [User_id] IS NOT NULL;
                IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Subscriptions_Client_id' AND object_id = OBJECT_ID('Subscriptions'))
                    CREATE INDEX [IX_Subscriptions_Client_id] ON [Subscriptions] ([Client_id]);
                IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Subscriptions_Group_id' AND object_id = OBJECT_ID('Subscriptions'))
                    CREATE INDEX [IX_Subscriptions_Group_id] ON [Subscriptions] ([Group_id]);
                IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Registrations_Client_id' AND object_id = OBJECT_ID('Registrations'))
                    CREATE INDEX [IX_Registrations_Client_id] ON [Registrations] ([Client_id]);
                IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Registrations_Group_id' AND object_id = OBJECT_ID('Registrations'))
                    CREATE INDEX [IX_Registrations_Group_id] ON [Registrations] ([Group_id]);
                IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Clients_User_id' AND object_id = OBJECT_ID('Clients'))
                    CREATE UNIQUE INDEX [IX_Clients_User_id] ON [Clients] ([User_id]) WHERE [User_id] IS NOT NULL;
            ");

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
        }
    }
}