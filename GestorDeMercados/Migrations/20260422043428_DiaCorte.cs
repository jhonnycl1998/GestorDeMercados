using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GestorDeMercados.Migrations
{
    /// <summary>
    /// Migración: Optimización de la lógica de facturación de servicios.
    /// Cambia el formato de fecha fija por un día del mes recurrente.
    /// </summary>
    public partial class DiaCorte : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Elimina la columna 'FechaCorte' que obligaba a ingresar una fecha completa (día, mes, año).
            migrationBuilder.DropColumn(
                name: "FechaCorte",
                table: "Servicios");

            // Agrega la nueva columna 'DiaCorte' como un entero (int).
            // Esto permite que el sistema sepa que, por ejemplo, "todos los 15 de cada mes" vence el servicio.
            migrationBuilder.AddColumn<int>(
                name: "DiaCorte",
                table: "Servicios",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Revierte el cambio: elimina el entero y restaura el tipo DateTime original.
            migrationBuilder.DropColumn(
                name: "DiaCorte",
                table: "Servicios");

            migrationBuilder.AddColumn<DateTime>(
                name: "FechaCorte",
                table: "Servicios",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }
    }
}