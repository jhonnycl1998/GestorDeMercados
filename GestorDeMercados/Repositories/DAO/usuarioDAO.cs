using Microsoft.EntityFrameworkCore;
using GestorDeMercados.Data;
using GestorDeMercados.Models;
using GestorDeMercados.Repositories.Interfaces;

namespace GestorDeMercados.Repositories.DAO
{
    // Clase que gestiona las credenciales y perfiles de acceso
    public class usuarioDAO : IUsuario
    {
        private readonly APPDbContext _context;

        public usuarioDAO(APPDbContext context)
        {
            _context = context;
        }

        // Lista todos los usuarios cargando su Rol (Admin/Dueño)
        // Usa .Include(u => u.Rol) para saber qué permisos tiene cada cuenta
        public IEnumerable<Usuario> Listar()
        {
            return _context.Usuarios
                .Include(u => u.Rol)
                .ToList();
        }

        // Busca un usuario por su ID y trae su Rol asociado
        public Usuario? Buscar(int id)
        {
            return _context.Usuarios
                .Include(u => u.Rol)
                .FirstOrDefault(u => u.id == id);
        }

        // Registra un nuevo usuario (aquí es donde se guarda el hash de la contraseña)
        public void Registrar(Usuario usuario)
        {
            _context.Usuarios.Add(usuario);
            _context.SaveChanges();
        }

        // Actualiza datos del usuario (como cambio de clave o de rol)
        public void Actualizar(Usuario usuario)
        {
            _context.Usuarios.Update(usuario);
            _context.SaveChanges();
        }

        // Elimina una cuenta de usuario del sistema
        public void Eliminar(int id)
        {
            var usuario = _context.Usuarios.Find(id);

            if (usuario != null)
            {
                _context.Usuarios.Remove(usuario);
                _context.SaveChanges();
            }
        }

        // MÉTODO PARA EL LOGIN:
        // Busca al usuario por su nombre de cuenta (string) para validar el inicio de sesión.
        public Usuario? ObtenerPorUsuario(string usuario)
        {
            return 

                _context.Usuarios
                .Include(u=> u.Rol)
                .Include(u=>u.Dueno)
                .FirstOrDefault(u => u.usuario == usuario);
        }
    }
}