using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace dance.API.Migrations
{
    /// <inheritdoc />
    public partial class AddUserTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    User_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Role = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    Client_id = table.Column<int>(type: "int", nullable: true),
                    Client_id1 = table.Column<int>(type: "int", nullable: true),
                    Trainer_id = table.Column<int>(type: "int", nullable: true),
                    Trainer_id1 = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.User_id);
                    table.ForeignKey(
                        name: "FK_Users_Clients_Client_id1",
                        column: x => x.Client_id1,
                        principalTable: "Clients",
                        principalColumn: "Client_id");
                    table.ForeignKey(
                        name: "FK_Users_Trainers_Trainer_id1",
                        column: x => x.Trainer_id1,
                        principalTable: "Trainers",
                        principalColumn: "Trainer_id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Users_Client_id1",
                table: "Users",
                column: "Client_id1");

            migrationBuilder.CreateIndex(
                name: "IX_Users_Trainer_id1",
                table: "Users",
                column: "Trainer_id1");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
