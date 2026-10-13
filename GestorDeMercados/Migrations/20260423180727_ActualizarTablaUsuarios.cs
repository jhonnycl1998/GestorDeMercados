using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GestorDeMercados.Migrations
{
    /// <summary>
    /// Migración de refactorización y seguridad: 
    /// Ajusta la nomenclatura técnica de usuarios y refuerza la integridad de pagos.
    /// </summary>
    public partial class ActualizarTablaUsuarios : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Paso 1: Eliminamos temporalmente la relación para modificar sus reglas de negocio.
            migrationBuilder.DropForeignKey(
                name: "FK_Pagos_Deudas_DeudaId",
                table: "Pagos");

            // Paso 2: Estandarización de seguridad.
            // Cambia 'contraseñaHash' a 'contrasenaHash' para evitar caracteres especiales (ñ)
            // en la base de datos, garantizando compatibilidad total con el servidor SQL.
            migrationBuilder.RenameColumn(
                name: "contraseñaHash",
                table: "Usuarios",
                newName: "contrasenaHash");

            // Paso 3: Cambio de regla de integridad (De Cascade a Restrict).
            // IMPORTANTE: Ahora el sistema NO permitirá borrar una Deuda si ya tiene un Pago asociado.
            // Esto protege la contabilidad del sistema para que no queden pagos "huérfanos".
            migrationBuilder.AddForeignKey(
                name: "FK_Pagos_Deudas_DeudaId",
                table: "Pagos",
                column: "DeudaId",
                principalTable: "Deudas",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Reversión de los cambios: restaura el borrado en cascada y la nomenclatura original.
            migrationBuilder.DropForeignKey(
                name: "FK_Pagos_Deudas_DeudaId",
                table: "Pagos");

            migrationBuilder.RenameColumn(
                name: "contrasenaHash",
                table: "Usuarios",
                newName: "contraseñaHash");

            migrationBuilder.AddForeignKey(
                name: "FK_Pagos_Deudas_DeudaId",
                table: "Pagos",
                column: "DeudaId",
                principalTable: "Deudas",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}