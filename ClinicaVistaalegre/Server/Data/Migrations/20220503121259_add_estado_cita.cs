using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ClinicaVistaalegre.Server.Data.Migrations
{
    public partial class add_estado_cita : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Estado",
                table: "Citas",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Estado",
                table: "Citas");
        }
    }
}
