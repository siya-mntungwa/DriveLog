using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DriveLog.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddDriveLogDetails : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "purpose",
                table: "DriveLogs",
                newName: "Purpose");

            migrationBuilder.AddColumn<string>(
                name: "Customer",
                table: "DriveLogs",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "DeliveryNoteId",
                table: "DriveLogs",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "EndKm",
                table: "DriveLogs",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "StartKm",
                table: "DriveLogs",
                type: "int",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Customer",
                table: "DriveLogs");

            migrationBuilder.DropColumn(
                name: "DeliveryNoteId",
                table: "DriveLogs");

            migrationBuilder.DropColumn(
                name: "EndKm",
                table: "DriveLogs");

            migrationBuilder.DropColumn(
                name: "StartKm",
                table: "DriveLogs");

            migrationBuilder.RenameColumn(
                name: "Purpose",
                table: "DriveLogs",
                newName: "purpose");
        }
    }
}
