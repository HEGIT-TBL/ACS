using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ACS.Core.Migrations
{
    public partial class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AccessPoints",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    AccessPointType = table.Column<int>(type: "integer", nullable: false),
                    Location = table.Column<string>(type: "text", nullable: false),
                    ControllerIP = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AccessPoints", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: true),
                    Surname = table.Column<string>(type: "text", nullable: false),
                    Patronymic = table.Column<string>(type: "text", nullable: true),
                    ProfilePicture = table.Column<byte[]>(type: "bytea", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Cameras",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Location = table.Column<string>(type: "text", nullable: false),
                    StreamLink = table.Column<string>(type: "text", nullable: false),
                    AccessPointId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cameras", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Cameras_AccessPoints_AccessPointId",
                        column: x => x.AccessPointId,
                        principalTable: "AccessPoints",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "AccessEvents",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CaptureTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsPermissionGranted = table.Column<bool>(type: "boolean", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: true),
                    AccessPointId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AccessEvents", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AccessEvents_AccessPoints_AccessPointId",
                        column: x => x.AccessPointId,
                        principalTable: "AccessPoints",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_AccessEvents_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Cars",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CarNumberPlate = table.Column<string>(type: "text", nullable: false),
                    CarModel = table.Column<string>(type: "text", nullable: true),
                    Color = table.Column<string>(type: "text", nullable: true),
                    UserId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cars", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Cars_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Identifiers",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Photo = table.Column<byte[]>(type: "bytea", nullable: true),
                    FacePoints = table.Column<float[]>(type: "real[]", nullable: true),
                    UserId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Identifiers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Identifiers_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "KeyCards",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    KeyCardId = table.Column<string>(type: "text", nullable: false),
                    ExpirationDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    OwnerId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_KeyCards", x => x.Id);
                    table.ForeignKey(
                        name: "FK_KeyCards_Users_OwnerId",
                        column: x => x.OwnerId,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "FaceRecognizedEvents",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Probability = table.Column<double>(type: "double precision", nullable: false),
                    CaptureTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    RecognizedUserId = table.Column<Guid>(type: "uuid", nullable: true),
                    CameraId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FaceRecognizedEvents", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FaceRecognizedEvents_Cameras_CameraId",
                        column: x => x.CameraId,
                        principalTable: "Cameras",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_FaceRecognizedEvents_Users_RecognizedUserId",
                        column: x => x.RecognizedUserId,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "ParkingLots",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    LotNumber = table.Column<int>(type: "integer", nullable: false),
                    State = table.Column<int>(type: "integer", nullable: false),
                    PlacedCarId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ParkingLots", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ParkingLots_Cars_PlacedCarId",
                        column: x => x.PlacedCarId,
                        principalTable: "Cars",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "AccessPointKeyCard",
                columns: table => new
                {
                    AllowedKeyCardsId = table.Column<Guid>(type: "uuid", nullable: false),
                    AvailableAccessPointsId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AccessPointKeyCard", x => new { x.AllowedKeyCardsId, x.AvailableAccessPointsId });
                    table.ForeignKey(
                        name: "FK_AccessPointKeyCard_AccessPoints_AvailableAccessPointsId",
                        column: x => x.AvailableAccessPointsId,
                        principalTable: "AccessPoints",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AccessPointKeyCard_KeyCards_AllowedKeyCardsId",
                        column: x => x.AllowedKeyCardsId,
                        principalTable: "KeyCards",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ParkingLotStateChangedEvents",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    StateChangeTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ChangedLotId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ParkingLotStateChangedEvents", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ParkingLotStateChangedEvents_ParkingLots_ChangedLotId",
                        column: x => x.ChangedLotId,
                        principalTable: "ParkingLots",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_AccessEvents_AccessPointId",
                table: "AccessEvents",
                column: "AccessPointId");

            migrationBuilder.CreateIndex(
                name: "IX_AccessEvents_UserId",
                table: "AccessEvents",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AccessPointKeyCard_AvailableAccessPointsId",
                table: "AccessPointKeyCard",
                column: "AvailableAccessPointsId");

            migrationBuilder.CreateIndex(
                name: "IX_Cameras_AccessPointId",
                table: "Cameras",
                column: "AccessPointId");

            migrationBuilder.CreateIndex(
                name: "IX_Cars_UserId",
                table: "Cars",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_FaceRecognizedEvents_CameraId",
                table: "FaceRecognizedEvents",
                column: "CameraId");

            migrationBuilder.CreateIndex(
                name: "IX_FaceRecognizedEvents_RecognizedUserId",
                table: "FaceRecognizedEvents",
                column: "RecognizedUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Identifiers_UserId",
                table: "Identifiers",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_KeyCards_OwnerId",
                table: "KeyCards",
                column: "OwnerId");

            migrationBuilder.CreateIndex(
                name: "IX_ParkingLots_PlacedCarId",
                table: "ParkingLots",
                column: "PlacedCarId");

            migrationBuilder.CreateIndex(
                name: "IX_ParkingLotStateChangedEvents_ChangedLotId",
                table: "ParkingLotStateChangedEvents",
                column: "ChangedLotId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AccessEvents");

            migrationBuilder.DropTable(
                name: "AccessPointKeyCard");

            migrationBuilder.DropTable(
                name: "FaceRecognizedEvents");

            migrationBuilder.DropTable(
                name: "Identifiers");

            migrationBuilder.DropTable(
                name: "ParkingLotStateChangedEvents");

            migrationBuilder.DropTable(
                name: "KeyCards");

            migrationBuilder.DropTable(
                name: "Cameras");

            migrationBuilder.DropTable(
                name: "ParkingLots");

            migrationBuilder.DropTable(
                name: "AccessPoints");

            migrationBuilder.DropTable(
                name: "Cars");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
