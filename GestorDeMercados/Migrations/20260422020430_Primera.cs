using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GestorDeMercados.Migrations
{
    /// <summary>
    /// Representa la creación inicial de la base de datos "MercaditoDonPepe".
    /// Define la estructura, tipos de datos y relaciones de integridad.
    /// </summary>
    public partial class Primera : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Creación de la tabla de Roles (Administrador, Dueño)
            migrationBuilder.CreateTable(
                name: "Roles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"), // Autoincremental
                    Nombre = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Roles", x => x.Id);
                });

            // Definición de Servicios con precisión decimal para evitar pérdida de céntimos
            migrationBuilder.CreateTable(
                name: "Servicios",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Costo = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    FechaCorte = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Servicios", x => x.Id);
                });

            // Tabla de Usuarios vinculada a Roles mediante una Llave Foránea (FK)
            migrationBuilder.CreateTable(
                name: "Usuarios",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    usuario = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    contraseñaHash = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RolId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Usuarios", x => x.id);
                    table.ForeignKey(
                        name: "FK_Usuarios_Roles_RolId",
                        column: x => x.RolId,
                        principalTable: "Roles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade); // Si se borra el rol, se borra el usuario
                });

            // Entidad Dueño vinculada a Usuario (Relación 1 a 1 para seguridad)
            migrationBuilder.CreateTable(
                name: "Duenos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    nombre = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    apellido = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ruc = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    fechaNacimiento = table.Column<DateTime>(type: "datetime2", nullable: false),
                    direccion = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UsuarioId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Duenos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Duenos_Usuarios_UsuarioId",
                        column: x => x.UsuarioId,
                        principalTable: "Usuarios",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            // Tabla Tiendas: Puede o no tener un Dueño asignado inicialmente (DuenoId nullable)
            migrationBuilder.CreateTable(
                name: "Tiendas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TieneDueño = table.Column<bool>(type: "bit", nullable: false),
                    alquiler = table.Column<bool>(type: "bit", nullable: false),
                    costoAlquiler = table.Column<double>(type: "float", nullable: false),
                    DuenoId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tiendas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Tiendas_Duenos_DuenoId",
                        column: x => x.DuenoId,
                        principalTable: "Duenos",
                        principalColumn: "Id");
                });

            // Registro de Deudas con doble FK: Tienda y Servicio
            migrationBuilder.CreateTable(
                name: "Deudas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MontoDeuda = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    FechaFacturacion = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FechaLimitePago = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Estado = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    TiendaId = table.Column<int>(type: "int", nullable: false),
                    ServicioId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Deudas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Deudas_Servicios_ServicioId",
                        column: x => x.ServicioId,
                        principalTable: "Servicios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Deudas_Tiendas_TiendaId",
                        column: x => x.TiendaId,
                        principalTable: "Tiendas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            // Tabla de Pagos: Relacionada 1 a 1 con Deudas
            migrationBuilder.CreateTable(
                name: "Pagos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FechaPago = table.Column<DateTime>(type: "datetime2", nullable: false),
                    MontoPago = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    DeudaId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Pagos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Pagos_Deudas_DeudaId",
                        column: x => x.DeudaId,
                        principalTable: "Deudas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            // Creación de índices para optimizar la velocidad de búsqueda en consultas frecuentes
            migrationBuilder.CreateIndex(name: "IX_Deudas_ServicioId", table: "Deudas", column: "ServicioId");
            migrationBuilder.CreateIndex(name: "IX_Deudas_TiendaId", table: "Deudas", column: "TiendaId");
            migrationBuilder.CreateIndex(name: "IX_Duenos_UsuarioId", table: "Duenos", column: "UsuarioId", unique: true);
            migrationBuilder.CreateIndex(name: "IX_Pagos_DeudaId", table: "Pagos", column: "DeudaId", unique: true);
            migrationBuilder.CreateIndex(name: "IX_Tiendas_DuenoId", table: "Tiendas", column: "DuenoId");
            migrationBuilder.CreateIndex(name: "IX_Usuarios_RolId", table: "Usuarios", column: "RolId");
        }

        /// <summary>
        /// Método para revertir la migración (borrar las tablas) en caso de error.
        /// </summary>
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(name: "Pagos");
            migrationBuilder.DropTable(name: "Deudas");
            migrationBuilder.DropTable(name: "Servicios");
            migrationBuilder.DropTable(name: "Tiendas");
            migrationBuilder.DropTable(name: "Duenos");
            migrationBuilder.DropTable(name: "Usuarios");
            migrationBuilder.DropTable(name: "Roles");
        }
    }
}