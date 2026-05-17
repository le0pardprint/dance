using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace dance.API.Migrations
{
    public partial class FixAllRelationships : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Безопасное удаление foreign keys
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
                IF EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_Groups_Directions_Direction_id')
                    ALTER TABLE [Groups] DROP CONSTRAINT [FK_Groups_Directions_Direction_id];
                IF EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_Groups_Trainers_Trainer_id')
                    ALTER TABLE [Groups] DROP CONSTRAINT [FK_Groups_Trainers_Trainer_id];
            ");

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

            // Безопасное добавление колонок
            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT * FROM sys.columns WHERE name = 'User_id' AND object_id = OBJECT_ID('Trainers'))
                    ALTER TABLE [Trainers] ADD [User_id] int NULL;
                IF NOT EXISTS (SELECT * FROM sys.columns WHERE name = 'CreatedAt' AND object_id = OBJECT_ID('Subscriptions'))
                    ALTER TABLE [Subscriptions] ADD [CreatedAt] datetime2 NULL;
                IF NOT EXISTS (SELECT * FROM sys.columns WHERE name = 'User_id' AND object_id = OBJECT_ID('Clients'))
                    ALTER TABLE [Clients] ADD [User_id] int NULL;
            ");

            // Безопасное создание индексов
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

            // Безопасное создание foreign keys
            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_Clients_Users_User_id')
                    ALTER TABLE [Clients] ADD CONSTRAINT [FK_Clients_Users_User_id] FOREIGN KEY ([User_id]) REFERENCES [Users] ([User_id]) ON DELETE NO ACTION;
                IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_Groups_Directions_Direction_id')
                    ALTER TABLE [Groups] ADD CONSTRAINT [FK_Groups_Directions_Direction_id] FOREIGN KEY ([Direction_id]) REFERENCES [Directions] ([Direction_id]) ON DELETE NO ACTION;
                IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_Groups_Trainers_Trainer_id')
                    ALTER TABLE [Groups] ADD CONSTRAINT [FK_Groups_Trainers_Trainer_id] FOREIGN KEY ([Trainer_id]) REFERENCES [Trainers] ([Trainer_id]) ON DELETE NO ACTION;
                IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_Registrations_Clients_Client_id')
                    ALTER TABLE [Registrations] ADD CONSTRAINT [FK_Registrations_Clients_Client_id] FOREIGN KEY ([Client_id]) REFERENCES [Clients] ([Client_id]) ON DELETE NO ACTION;
                IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_Registrations_Groups_Group_id')
                    ALTER TABLE [Registrations] ADD CONSTRAINT [FK_Registrations_Groups_Group_id] FOREIGN KEY ([Group_id]) REFERENCES [Groups] ([Group_id]) ON DELETE NO ACTION;
                IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_Subscriptions_Clients_Client_id')
                    ALTER TABLE [Subscriptions] ADD CONSTRAINT [FK_Subscriptions_Clients_Client_id] FOREIGN KEY ([Client_id]) REFERENCES [Clients] ([Client_id]) ON DELETE NO ACTION;
                IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_Subscriptions_Groups_Group_id')
                    ALTER TABLE [Subscriptions] ADD CONSTRAINT [FK_Subscriptions_Groups_Group_id] FOREIGN KEY ([Group_id]) REFERENCES [Groups] ([Group_id]) ON DELETE NO ACTION;
                IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_Trainers_Users_User_id')
                    ALTER TABLE [Trainers] ADD CONSTRAINT [FK_Trainers_Users_User_id] FOREIGN KEY ([User_id]) REFERENCES [Users] ([User_id]) ON DELETE NO ACTION;
            ");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                IF EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_Clients_Users_User_id')
                    ALTER TABLE [Clients] DROP CONSTRAINT [FK_Clients_Users_User_id];
                IF EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_Groups_Directions_Direction_id')
                    ALTER TABLE [Groups] DROP CONSTRAINT [FK_Groups_Directions_Direction_id];
                IF EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_Groups_Trainers_Trainer_id')
                    ALTER TABLE [Groups] DROP CONSTRAINT [FK_Groups_Trainers_Trainer_id];
                IF EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_Registrations_Clients_Client_id')
                    ALTER TABLE [Registrations] DROP CONSTRAINT [FK_Registrations_Clients_Client_id];
                IF EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_Registrations_Groups_Group_id')
                    ALTER TABLE [Registrations] DROP CONSTRAINT [FK_Registrations_Groups_Group_id];
                IF EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_Subscriptions_Clients_Client_id')
                    ALTER TABLE [Subscriptions] DROP CONSTRAINT [FK_Subscriptions_Clients_Client_id];
                IF EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_Subscriptions_Groups_Group_id')
                    ALTER TABLE [Subscriptions] DROP CONSTRAINT [FK_Subscriptions_Groups_Group_id];
                IF EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_Trainers_Users_User_id')
                    ALTER TABLE [Trainers] DROP CONSTRAINT [FK_Trainers_Users_User_id];
            ");

            migrationBuilder.Sql(@"
                IF EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Trainers_User_id' AND object_id = OBJECT_ID('Trainers'))
                    DROP INDEX [IX_Trainers_User_id] ON [Trainers];
                IF EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Subscriptions_Client_id' AND object_id = OBJECT_ID('Subscriptions'))
                    DROP INDEX [IX_Subscriptions_Client_id] ON [Subscriptions];
                IF EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Subscriptions_Group_id' AND object_id = OBJECT_ID('Subscriptions'))
                    DROP INDEX [IX_Subscriptions_Group_id] ON [Subscriptions];
                IF EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Registrations_Client_id' AND object_id = OBJECT_ID('Registrations'))
                    DROP INDEX [IX_Registrations_Client_id] ON [Registrations];
                IF EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Registrations_Group_id' AND object_id = OBJECT_ID('Registrations'))
                    DROP INDEX [IX_Registrations_Group_id] ON [Registrations];
                IF EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Clients_User_id' AND object_id = OBJECT_ID('Clients'))
                    DROP INDEX [IX_Clients_User_id] ON [Clients];
            ");

            migrationBuilder.Sql(@"
                IF EXISTS (SELECT * FROM sys.columns WHERE name = 'User_id' AND object_id = OBJECT_ID('Trainers'))
                    ALTER TABLE [Trainers] DROP COLUMN [User_id];
                IF EXISTS (SELECT * FROM sys.columns WHERE name = 'CreatedAt' AND object_id = OBJECT_ID('Subscriptions'))
                    ALTER TABLE [Subscriptions] DROP COLUMN [CreatedAt];
                IF EXISTS (SELECT * FROM sys.columns WHERE name = 'User_id' AND object_id = OBJECT_ID('Clients'))
                    ALTER TABLE [Clients] DROP COLUMN [User_id];
            ");
        }
    }
}