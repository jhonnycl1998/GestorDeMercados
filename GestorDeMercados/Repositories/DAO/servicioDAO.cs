using GestorDeMercados.Data;
using GestorDeMercados.Models;
using GestorDeMercados.Repositories.Interfaces;

namespace GestorDeMercados.Repositories.DAO
{
    // Clase que gestiona el catálogo de servicios (Luz, Agua, Arbitrios, etc.)
    public class servicioDAO : IServicio
    {
        private readonly APPDbContext _context;

        public servicioDAO(APPDbContext context)
        {
            _context = context;
        }

        // Obtiene todos los tipos de servicios configurados en el sistema
        public IEnumerable<Servicio> Listar()
        {
            return _context.Servicios.ToList();
        }

        // Busca un servicio por su ID (ej. ID 1 para "Luz Eléctrica")
        public Servicio? Buscar(int id)
        {
            return _context.Servicios.Find(id);
        }

        // Crea un nuevo servicio con su nombre y costo base
        public void Registrar(Servicio servicio)
        {
            _context.Servicios.Add(servicio);
            _context.SaveChanges();
        }

        // Actualiza datos del servicio, como un cambio en la tarifa o día de corte
        public void Actualizar(Servicio servicio)
        {
            _context.Servicios.Update(servicio);
            _context.SaveChanges();
        }

        // Elimina un servicio si ya no se va a cobrar más en el mercadito
        public void Eliminar(int id)
        {
            var servicio = _context.Servicios.Find(id);

            if (servicio != null)
            {
                _context.Servicios.Remove(servicio);
                _context.SaveChanges();
            }
        }
    }
}