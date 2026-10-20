using GestorDeMercados.Data;
using GestorDeMercados.Models;
using GestorDeMercados.Repositories.Interfaces;

namespace GestorDeMercados.Repositories.DAO
{
    // Clase encargada de la persistencia y gestión de los locales (Tiendas)
    public class tiendaDAO : ITienda
    {
        private readonly APPDbContext _context;

        public tiendaDAO(APPDbContext context)
        {
            _context = context;
        }

        // Obtiene la lista de todos los locales registrados en el sistema
        public IEnumerable<Tienda> Listar()
        {
            return _context.Tiendas.ToList();
        }

        // Busca una tienda específica por su número de ID
        public Tienda? Buscar(int id)
        {
            return _context.Tiendas.Find(id);
        }

        // Registra un nuevo local en el mercadito (ej. Stand 101)
        public void Registrar(Tienda tienda)
        {
            _context.Tiendas.Add(tienda);
            _context.SaveChanges(); // Sincroniza con la base de datos SQL Server
        }

        // Actualiza la información del local (ej. si cambió de dueño o de costo de alquiler)
        public void Actualizar(Tienda tienda)
        {
            var existente = _context.Tiendas.Find(tienda.Id);
            if (existente == null)
                throw new Exception("La tienda no existe.");

            _context.Entry(existente).CurrentValues.SetValues(tienda);
            _context.SaveChanges();
        }

        // Elimina un local del registro del sistema
        public void Eliminar(int id)
        {
            var tienda = _context.Tiendas.Find(id);

            if (tienda != null)
            {
                _context.Tiendas.Remove(tienda);
                _context.SaveChanges();
            }
        }
    }
}