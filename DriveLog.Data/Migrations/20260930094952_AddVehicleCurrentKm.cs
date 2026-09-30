using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DriveLog.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddVehicleCurrentKm : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CurrentKm",
                table: "Vehicles",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CurrentKm",
                table: "Vehicles");
        }
    }
}
