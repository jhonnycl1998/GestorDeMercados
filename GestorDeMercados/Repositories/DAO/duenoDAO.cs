using Microsoft.EntityFrameworkCore;
using GestorDeMercados.Data;
using GestorDeMercados.Models;
using GestorDeMercados.Repositories.Interfaces;

namespace GestorDeMercados.Repositories.DAO
{
    // Clase encargada del acceso a datos para la entidad Dueno
    public class duenoDAO : IDueno
    {
        private readonly APPDbContext _context;

        public duenoDAO(APPDbContext context)
        {
            _context = context;
        }

        // Obtiene la lista de dueños incluyendo sus tiendas asociadas
        public IEnumerable<Dueno> Listar()
        {
            return _context.Duenos
                .Include(d => d.Tiendas) // Eager Loading: Carga la lista de locales de cada dueño
                .ToList();
        }

        // Busca un dueño específico y sus tiendas por su ID
        public Dueno? Buscar(int id)
        {
            return _context.Duenos
                .Include(d => d.Tiendas) // Asegura que al ver el perfil del dueño aparezcan sus tiendas
                .FirstOrDefault(d => d.Id == id);
        }

        // Registra un nuevo dueño en el sistema
        public void Registrar(Dueno dueno)
        {
            _context.Duenos.Add(dueno);
            _context.SaveChanges(); // Persistencia en SQL Server
        }

        // Actualiza datos personales (ej. nueva dirección o teléfono)
        public void Actualizar(Dueno dueno)
        {
            _context.Duenos.Update(dueno);
            _context.SaveChanges();
        }

        // Elimina un registro de dueño de la base de datos
        public void Eliminar(int id)
        {
            var dueno = _context.Duenos.Find(id);

            if (dueno != null)
            {
                _context.Duenos.Remove(dueno);
                _context.SaveChanges();
            }
        }
    }
}