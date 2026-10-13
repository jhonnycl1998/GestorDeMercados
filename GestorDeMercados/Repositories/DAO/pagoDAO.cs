using Microsoft.EntityFrameworkCore;
using GestorDeMercados.Data;
using GestorDeMercados.Models;
using GestorDeMercados.Repositories.Interfaces;

namespace GestorDeMercados.Repositories.DAO
{
    public class pagoDAO : IPago
    {
        private readonly APPDbContext _context;

        // Inyección de dependencias para usar el contexto de la base de datos
        public pagoDAO(APPDbContext context)
        {
            _context = context;
        }

        // Obtiene todos los pagos incluyendo la información de la deuda relacionada
        public IEnumerable<Pago> Listar()
        {
            return _context.Pagos
                .Include(p => p.Deuda)
                .ToList();
        }

        // Reporte de ingresos del día: Carga en cadena Pago -> Deuda -> Tienda -> Dueño
        // Usa ThenInclude para navegar por relaciones multinivel y mostrar quién pagó hoy
        public IEnumerable<Pago> ObtenerPagosDeHoy()
        {
            return _context.Pagos
                .Include(p => p.Deuda)
                    .ThenInclude(d => d.Tienda)
                        .ThenInclude(t => t.Dueno)
                .Include(p => p.Deuda)
                    .ThenInclude(d => d.Servicio)
                .Where(p => p.FechaPago.Date == DateTime.Now.Date) // Filtra por la fecha actual del servidor
                .ToList();
        }

        // Obtiene un pago por ID con todos sus detalles para generar el comprobante (Voucher)
        public Pago? ObtenerPorId(int id)
        {
            return _context.Pagos
                .Include(p => p.Deuda)
                    .ThenInclude(d => d.Tienda)
                        .ThenInclude(t => t.Dueno)
                .Include(p => p.Deuda)
                    .ThenInclude(d => d.Servicio)
                .FirstOrDefault(p => p.Id == id);
        }

        // Verifica si una deuda específica ya tiene un pago asociado
        public Pago? ObtenerPorDeudaId(int deudaId)
        {
            return _context.Pagos
                .Include(p => p.Deuda)
                .FirstOrDefault(p => p.DeudaId == deudaId);
        }

        // PROCESO CRÍTICO: Registro de pago con transacción ACID
        public Pago RegistrarPago(Pago pago)
        {
            // Inicia una transacción: O se guardan todos los cambios o ninguno
            using var transaction = _context.Database.BeginTransaction();

            try
            {
                var deuda = _context.Deudas.FirstOrDefault(d => d.Id == pago.DeudaId);

                // --- VALIDACIONES DE REGLA DE NEGOCIO ---
                if (deuda == null) throw new Exception("La deuda no existe.");
                if (deuda.Estado.ToLower() == "pagado") throw new Exception("La deuda ya fue pagada.");

                // Evita duplicidad de pagos para la misma deuda
                var pagoExistente = _context.Pagos.FirstOrDefault(p => p.DeudaId == pago.DeudaId);
                if (pagoExistente != null) throw new Exception("Ya existe un pago registrado para esta deuda.");

                // Valida que el monto sea exacto (ni más, ni menos)
                if (pago.MontoPago != deuda.MontoDeuda)
                    throw new Exception("El monto del pago debe ser igual al monto total de la deuda.");

                pago.FechaPago = DateTime.Now;

                // 1. Agrega el registro de pago a la tabla Pagos
                _context.Pagos.Add(pago);

                // 2. Actualiza el estado de la Deuda a 'Pagado'
                deuda.Estado = "Pagado";
                _context.Deudas.Update(deuda);

                // Guarda ambos cambios en SQL Server
                _context.SaveChanges();

                // Confirma la operación (Commit)
                transaction.Commit();

                return pago;
            }
            catch
            {
                // Si algo falla, deshace todo para evitar datos corruptos (Rollback)
                transaction.Rollback();
                throw;
            }
        }

        // Elimina un registro de pago por ID
        public bool Eliminar(int id)
        {
            var pago = _context.Pagos.FirstOrDefault(p => p.Id == id);

            if (pago == null) return false;

            _context.Pagos.Remove(pago);
            _context.SaveChanges();

            return true;
        }
    }
}