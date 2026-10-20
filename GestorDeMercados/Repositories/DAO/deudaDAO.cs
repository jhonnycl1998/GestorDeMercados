using Microsoft.EntityFrameworkCore;
using GestorDeMercados.Data;
using GestorDeMercados.Models;
using GestorDeMercados.Repositories.Interfaces;

namespace GestorDeMercados.Repositories.DAO
{
    // Implementación del Patrón DAO (Data Access Object) para la entidad Deuda
    public class deudaDAO : IDeuda
    {
        private readonly APPDbContext _context;

        public deudaDAO(APPDbContext context)
        {
            _context = context;
        }

        // Recupera todas las deudas registradas en la base de datos
        public IEnumerable<Deuda> Listar()
        {
            return _context.Deudas.ToList();
        }

        // Busca una deuda específica mediante su clave primaria
        public Deuda? Buscar(int id)
        {
            return _context.Deudas.Find(id);
        }

        // Persiste una nueva deuda (Ej: Cobro de luz del mes de abril)
        public void Registrar(Deuda deuda)
        {
            _context.Deudas.Add(deuda);
            _context.SaveChanges();
        }

        // Actualiza el estado o monto de una deuda existente
        public void Actualizar(Deuda deuda)
        {
            _context.Deudas.Update(deuda);
            _context.SaveChanges();
        }

        // Elimina lógicamente el registro si no tiene restricciones de integridad
        public void Eliminar(int id)
        {
            var deuda = _context.Deudas.Find(id);

            if (deuda != null)
            {
                _context.Deudas.Remove(deuda);
                _context.SaveChanges();
            }
        }

        // Lógica de Validación: Evita que se genere la misma deuda dos veces
        // Verifica si ya existe un cobro para la misma tienda, servicio, mes y año.
        public bool ExisteDeuda(int tiendaId, int servicioId, DateTime fechaFacturacion)
        {
            return _context.Deudas.Any(d =>
                d.TiendaId == tiendaId &&
                d.ServicioId == servicioId &&
                d.FechaFacturacion.Month == fechaFacturacion.Month &&
                d.FechaFacturacion.Year == fechaFacturacion.Year);
        }

        // Consulta de Negocio Avanzada:
        // Usa 'Include' (Eager Loading) para traer los datos de la Tienda y el Servicio en una sola consulta.
        // Filtra solo lo que el dueño tiene por pagar para mostrarlo en su panel.
        public IEnumerable<Deuda> ObtenerPendientesPorDueno(int duenoId)
        {
            return _context.Deudas
                .Include(d => d.Tienda)   // Carga los datos del local vinculado
                .Include(d => d.Servicio) // Carga el nombre del servicio (Luz/Agua)
                .Where(d => d.Estado == "Pendiente" && d.Tienda.DuenoId == duenoId)
                .ToList();
        }
    }
}