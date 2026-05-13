using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace dance.API.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Clients",
                columns: table => new
                {
                    Client_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LastName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FirstName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Age = table.Column<int>(type: "int", nullable: false),
                    Phone = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Clients", x => x.Client_id);
                });

            migrationBuilder.CreateTable(
                name: "Directions",
                columns: table => new
                {
                    Direction_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AgeGroup = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Level = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Directions", x => x.Direction_id);
                });

            migrationBuilder.CreateTable(
                name: "Trainers",
                columns: table => new
                {
                    Trainer_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LastName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FirstName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Direction_id = table.Column<int>(type: "int", nullable: false),
                    Phone = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Direction_id1 = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Trainers", x => x.Trainer_id);
                    table.ForeignKey(
                        name: "FK_Trainers_Directions_Direction_id1",
                        column: x => x.Direction_id1,
                        principalTable: "Directions",
                        principalColumn: "Direction_id");
                });

            migrationBuilder.CreateTable(
                name: "Groups",
                columns: table => new
                {
                    Group_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Direction_id = table.Column<int>(type: "int", nullable: false),
                    Trainer_id = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Groups", x => x.Group_id);
                    table.ForeignKey(
                        name: "FK_Groups_Directions_Direction_id",
                        column: x => x.Direction_id,
                        principalTable: "Directions",
                        principalColumn: "Direction_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Groups_Trainers_Trainer_id",
                        column: x => x.Trainer_id,
                        principalTable: "Trainers",
                        principalColumn: "Trainer_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Classes",
                columns: table => new
                {
                    Class_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Group_id = table.Column<int>(type: "int", nullable: false),
                    Trainer_id = table.Column<int>(type: "int", nullable: false),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Time = table.Column<TimeSpan>(type: "time", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Classes", x => x.Class_id);
                    table.ForeignKey(
                        name: "FK_Classes_Groups_Group_id",
                        column: x => x.Group_id,
                        principalTable: "Groups",
                        principalColumn: "Group_id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Classes_Trainers_Trainer_id",
                        column: x => x.Trainer_id,
                        principalTable: "Trainers",
                        principalColumn: "Trainer_id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Registrations",
                columns: table => new
                {
                    Registration_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Client_id = table.Column<int>(type: "int", nullable: false),
                    Group_id = table.Column<int>(type: "int", nullable: false),
                    Registration_date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Client_id1 = table.Column<int>(type: "int", nullable: true),
                    Group_id1 = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Registrations", x => x.Registration_id);
                    table.ForeignKey(
                        name: "FK_Registrations_Clients_Client_id1",
                        column: x => x.Client_id1,
                        principalTable: "Clients",
                        principalColumn: "Client_id");
                    table.ForeignKey(
                        name: "FK_Registrations_Groups_Group_id1",
                        column: x => x.Group_id1,
                        principalTable: "Groups",
                        principalColumn: "Group_id");
                });

            migrationBuilder.CreateTable(
                name: "Subscriptions",
                columns: table => new
                {
                    Sub_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Client_id = table.Column<int>(type: "int", nullable: false),
                    Group_id = table.Column<int>(type: "int", nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Client_id1 = table.Column<int>(type: "int", nullable: true),
                    Group_id1 = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Subscriptions", x => x.Sub_id);
                    table.ForeignKey(
                        name: "FK_Subscriptions_Clients_Client_id1",
                        column: x => x.Client_id1,
                        principalTable: "Clients",
                        principalColumn: "Client_id");
                    table.ForeignKey(
                        name: "FK_Subscriptions_Groups_Group_id1",
                        column: x => x.Group_id1,
                        principalTable: "Groups",
                        principalColumn: "Group_id");
                });

            migrationBuilder.CreateTable(
                name: "AttendanceRecords",
                columns: table => new
                {
                    Record_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Client_id = table.Column<int>(type: "int", nullable: false),
                    Class_id = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AttendanceRecords", x => x.Record_id);
                    table.ForeignKey(
                        name: "FK_AttendanceRecords_Classes_Class_id",
                        column: x => x.Class_id,
                        principalTable: "Classes",
                        principalColumn: "Class_id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AttendanceRecords_Clients_Client_id",
                        column: x => x.Client_id,
                        principalTable: "Clients",
                        principalColumn: "Client_id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AttendanceRecords_Class_id",
                table: "AttendanceRecords",
                column: "Class_id");

            migrationBuilder.CreateIndex(
                name: "IX_AttendanceRecords_Client_id",
                table: "AttendanceRecords",
                column: "Client_id");

            migrationBuilder.CreateIndex(
                name: "IX_Classes_Group_id",
                table: "Classes",
                column: "Group_id");

            migrationBuilder.CreateIndex(
                name: "IX_Classes_Trainer_id",
                table: "Classes",
                column: "Trainer_id");

            migrationBuilder.CreateIndex(
                name: "IX_Groups_Direction_id",
                table: "Groups",
                column: "Direction_id");

            migrationBuilder.CreateIndex(
                name: "IX_Groups_Trainer_id",
                table: "Groups",
                column: "Trainer_id");

            migrationBuilder.CreateIndex(
                name: "IX_Registrations_Client_id1",
                table: "Registrations",
                column: "Client_id1");

            migrationBuilder.CreateIndex(
                name: "IX_Registrations_Group_id1",
                table: "Registrations",
                column: "Group_id1");

            migrationBuilder.CreateIndex(
                name: "IX_Subscriptions_Client_id1",
                table: "Subscriptions",
                column: "Client_id1");

            migrationBuilder.CreateIndex(
                name: "IX_Subscriptions_Group_id1",
                table: "Subscriptions",
                column: "Group_id1");

            migrationBuilder.CreateIndex(
                name: "IX_Trainers_Direction_id1",
                table: "Trainers",
                column: "Direction_id1");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AttendanceRecords");

            migrationBuilder.DropTable(
                name: "Registrations");

            migrationBuilder.DropTable(
                name: "Subscriptions");

            migrationBuilder.DropTable(
                name: "Classes");

            migrationBuilder.DropTable(
                name: "Clients");

            migrationBuilder.DropTable(
                name: "Groups");

            migrationBuilder.DropTable(
                name: "Trainers");

            migrationBuilder.DropTable(
                name: "Directions");
        }
    }
}
