using GestorDeMercados.Data;
using GestorDeMercados.Models;
using GestorDeMercados.Repositories.Interfaces;

namespace GestorDeMercados.Repositories.DAO
{
    // Clase que gestiona los niveles de acceso (Roles) en la base de datos
    public class rolDAO : IRol
    {
        private readonly APPDbContext _context;

        public rolDAO(APPDbContext context)
        {
            _context = context;
        }

        // Obtiene la lista maestra de roles (ej. Admin, Dueño)
        public IEnumerable<Rol> Listar()
        {
            return _context.Roles.ToList();
        }

        // Busca un rol específico por su ID único
        public Rol? Buscar(int id)
        {
            return _context.Roles.Find(id);
        }

        // Registra un nuevo tipo de rol en el sistema
        public void Registrar(Rol rol)
        {
            _context.Roles.Add(rol);
            _context.SaveChanges(); // Confirma la inserción en SQL Server
        }

        // Permite modificar el nombre de un rol existente
        public void Actualizar(Rol rol)
        {
            _context.Roles.Update(rol);
            _context.SaveChanges();
        }

        // Elimina un rol del sistema
        public void Eliminar(int id)
        {
            var rol = _context.Roles.Find(id);

            if (rol != null)
            {
                // Remueve el registro y sincroniza los cambios
                _context.Roles.Remove(rol);
                _context.SaveChanges();
            }
        }
    }
}