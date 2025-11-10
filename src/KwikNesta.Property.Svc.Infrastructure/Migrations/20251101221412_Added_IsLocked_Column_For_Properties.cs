using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KwikNesta.Property.Svc.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Added_IsLocked_Column_For_Properties : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsLocked",
                schema: "property-svc",
                table: "Properties",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsLocked",
                schema: "property-svc",
                table: "Properties");
        }
    }
}
