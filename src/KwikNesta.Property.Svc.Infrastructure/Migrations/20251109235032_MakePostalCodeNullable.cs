using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KwikNesta.Property.Svc.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class MakePostalCodeNullable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsVerified",
                schema: "property-svc",
                table: "PropertyLocations",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsCoordinatesSent",
                schema: "property-svc",
                table: "Properties",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsOwnerShipVerified",
                schema: "property-svc",
                table: "Properties",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "StatusReasons",
                schema: "property-svc",
                table: "Properties",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "VerificationReasons",
                schema: "property-svc",
                table: "Properties",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsVerified",
                schema: "property-svc",
                table: "PropertyLocations");

            migrationBuilder.DropColumn(
                name: "IsCoordinatesSent",
                schema: "property-svc",
                table: "Properties");

            migrationBuilder.DropColumn(
                name: "IsOwnerShipVerified",
                schema: "property-svc",
                table: "Properties");

            migrationBuilder.DropColumn(
                name: "StatusReasons",
                schema: "property-svc",
                table: "Properties");

            migrationBuilder.DropColumn(
                name: "VerificationReasons",
                schema: "property-svc",
                table: "Properties");
        }
    }
}
