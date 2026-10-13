using GestorDeMercados.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace GestorDeMercados.Data
{
    public static class DbSeeder
    {
        public const string RucAsociacion = "00000000000";

        public static void Seed(APPDbContext db, IConfiguration config)
        {
            var hasher = new PasswordHasher<Usuario>();

            // Roles (si existen los anteriores, los renombra)
            var rolAdmin = db.Roles.FirstOrDefault(r => r.Nombre == "Admin")
                           ?? db.Roles.FirstOrDefault(r => r.Nombre == "Administrador");
            if (rolAdmin == null) rolAdmin = db.Roles.Add(new Rol { Nombre = "Admin" }).Entity;
            else rolAdmin.Nombre = "Admin";

            var rolUser = db.Roles.FirstOrDefault(r => r.Nombre == "User")
                          ?? db.Roles.FirstOrDefault(r => r.Nombre == "Dueño");
            if (rolUser == null) rolUser = db.Roles.Add(new Rol { Nombre = "User" }).Entity;
            else rolUser.Nombre = "User";

            db.SaveChanges();

            // Asociación: dueño por defecto y único Admin
            var nombre = config["Seed:AdminUsuario"] ?? "asociacion";
            var clave = config["Seed:AdminClave"] ?? "Admin123!";

            var asoc = db.Duenos.Include(d => d.Usuario)
                                .FirstOrDefault(d => d.ruc == RucAsociacion);
            if (asoc == null)
            {
                var u = new Usuario { usuario = nombre, RolId = rolAdmin.Id };
                u.contrasenaHash = hasher.HashPassword(u, clave);

                db.Duenos.Add(new Dueno
                {
                    nombre = "Asociación",
                    apellido = "del Mercado",
                    ruc = RucAsociacion,
                    fechaNacimiento = DateTime.Today,
                    direccion = "Mercado",
                    Usuario = u
                });
            }
            else if (asoc.Usuario != null)
            {
                asoc.Usuario.usuario = nombre;
                asoc.Usuario.RolId = rolAdmin.Id;
                asoc.Usuario.contrasenaHash = hasher.HashPassword(asoc.Usuario, clave);
            }

            db.SaveChanges();
        }
    }
}
