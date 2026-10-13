using Microsoft.AspNetCore.Identity;
using GestorDeMercados.Models;

namespace GestorDeMercados.Data
{
    public static class DbSeeder
    {
        public const string RucAsociacion = "00000000000";

        public static void Seed(APPDbContext db, IConfiguration config)
        {
            var hasher = new PasswordHasher<Usuario>();

            var rolAdmin = db.Roles.FirstOrDefault(r => r.Nombre == "Administrador")
                ?? db.Roles.Add(new Rol { Nombre = "Administrador" }).Entity;
            var rolDueno = db.Roles.FirstOrDefault(r => r.Nombre =="Dueño")
                ?? db.Roles.Add(new Rol { Nombre ="Dueño"}).Entity;
            db.SaveChanges();

            var adminNombre = config["Seed:AdminUsuario"] ?? "admin";
            if(!db.Usuarios.Any(u=> u.usuario == adminNombre))
            {
                var admin = new Usuario { usuario = adminNombre, RolId = rolAdmin.Id };
                admin.contrasenaHash = hasher.HashPassword(admin, config["Seed:AdminClave"] ?? "Admin123!");
                db.Usuarios.Add(admin);
            }

            if(!db.Duenos.Any(d=> d.ruc == RucAsociacion))
            {
                var uAsoc = new Usuario { usuario = "asociacion", RolId = rolDueno.Id };
                uAsoc.contrasenaHash = hasher.HashPassword(uAsoc, Guid.NewGuid().ToString());

                db.Duenos.Add(new Dueno
                {
                    nombre = "Asociación",
                    apellido = "del Mercado",
                    ruc = RucAsociacion,
                    fechaNacimiento = DateTime.Today,
                    direccion = "Mercado",
                    Usuario = uAsoc
                });
            }
            db.SaveChanges();
        }
    }
}
