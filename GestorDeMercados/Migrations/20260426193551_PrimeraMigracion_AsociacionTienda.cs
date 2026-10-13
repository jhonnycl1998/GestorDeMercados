using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GestorDeMercados.Migrations
{
    /// <summary>
    /// Migración de control: Preparación para la lógica de asociación automática de tiendas.
    /// </summary>
    public partial class PrimeraMigracion_AsociacionTienda : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Este método Up está diseñado para actualizaciones de esquema que vinculan
            // dinámicamente las tiendas con sus respectivos dueños.
            // Al estar vacío, indica que la estructura de llaves foráneas definida 
            // en la migración 'Primera' ya es suficiente para soportar la lógica de negocio.
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Revierte cualquier cambio manual realizado en la asociación de tiendas.
        }
    }
}