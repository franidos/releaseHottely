using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace nextadvisordotnet.DAL.Migrations
{
    /// <inheritdoc />
    public partial class om_20240326_2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RoomMapOrigin_Room_IdRoom",
                table: "RoomMapOrigin");

            migrationBuilder.RenameColumn(
                name: "IdRoom",
                table: "RoomMapOrigin",
                newName: "idRoom");

            migrationBuilder.RenameColumn(
                name: "IdRoomMap",
                table: "RoomMapOrigin",
                newName: "idRoomMap");

            migrationBuilder.RenameIndex(
                name: "IX_RoomMapOrigin_IdRoom",
                table: "RoomMapOrigin",
                newName: "IX_RoomMapOrigin_idRoom");

            migrationBuilder.AddColumn<string>(
                name: "channelName",
                table: "RoomMapOrigin",
                type: "varchar(50)",
                unicode: false,
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "idEstablishment",
                table: "RoomMapOrigin",
                type: "int",
                nullable: false,
                defaultValueSql: "1");

            migrationBuilder.AddColumn<string>(
                name: "user",
                table: "RoomMapOrigin",
                type: "varchar(50)",
                unicode: false,
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "treatment",
                table: "Guest",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_RoomMapOrigin_idEstablishment",
                table: "RoomMapOrigin",
                column: "idEstablishment");

            migrationBuilder.AddForeignKey(
                name: "FK_RoomMapOrigin_Room_idRoom",
                table: "RoomMapOrigin",
                column: "idRoom",
                principalTable: "Room",
                principalColumn: "idRoom");

            migrationBuilder.AddForeignKey(
                name: "FK_room_map_idEstablishment__403A8C7D",
                table: "RoomMapOrigin",
                column: "idEstablishment",
                principalTable: "Establishment",
                principalColumn: "idEstablishment",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RoomMapOrigin_Room_idRoom",
                table: "RoomMapOrigin");

            migrationBuilder.DropForeignKey(
                name: "FK_room_map_idEstablishment__403A8C7D",
                table: "RoomMapOrigin");

            migrationBuilder.DropIndex(
                name: "IX_RoomMapOrigin_idEstablishment",
                table: "RoomMapOrigin");

            migrationBuilder.DropColumn(
                name: "channelName",
                table: "RoomMapOrigin");

            migrationBuilder.DropColumn(
                name: "idEstablishment",
                table: "RoomMapOrigin");

            migrationBuilder.DropColumn(
                name: "user",
                table: "RoomMapOrigin");

            migrationBuilder.DropColumn(
                name: "treatment",
                table: "Guest");

            migrationBuilder.RenameColumn(
                name: "idRoom",
                table: "RoomMapOrigin",
                newName: "IdRoom");

            migrationBuilder.RenameColumn(
                name: "idRoomMap",
                table: "RoomMapOrigin",
                newName: "IdRoomMap");

            migrationBuilder.RenameIndex(
                name: "IX_RoomMapOrigin_idRoom",
                table: "RoomMapOrigin",
                newName: "IX_RoomMapOrigin_IdRoom");

            migrationBuilder.AddForeignKey(
                name: "FK_RoomMapOrigin_Room_IdRoom",
                table: "RoomMapOrigin",
                column: "IdRoom",
                principalTable: "Room",
                principalColumn: "idRoom");
        }
    }
}
