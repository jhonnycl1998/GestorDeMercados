using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GestorDeMercados.Migrations
{
    /// <summary>
    /// Migración de refactorización: Corrección de nomenclatura en la tabla Tiendas.
    /// </summary>
    public partial class CambioTieneDueno : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Renombra la columna 'TieneDueño' a 'TieneDueno' para estandarizar el uso de caracteres ASCII.
            // Esto evita posibles errores de colación o codificación en servidores SQL remotos o Azure.
            migrationBuilder.RenameColumn(
                name: "TieneDueño",
                table: "Tiendas",
                newName: "TieneDueno");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Método para revertir el cambio en caso de ser necesario volver a la versión anterior.
            migrationBuilder.RenameColumn(
                name: "TieneDueno",
                table: "Tiendas",
                newName: "TieneDueño");
        }
    }
}