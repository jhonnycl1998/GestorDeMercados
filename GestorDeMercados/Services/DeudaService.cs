using GestorDeMercados.Models;
using GestorDeMercados.Repositories.Interfaces;

namespace GestorDeMercados.Services
{
    // Clase de Servicio: Contiene la lógica de negocio compleja que coordina varios DAOs.
    public class DeudaService
    {
        private readonly ITienda _tiendaDao;
        private readonly IServicio _servicioDao;
        private readonly IDeuda _deudaDao;

        // Inyección de interfaces (Abstracción) para desacoplar la lógica de la base de datos
        public DeudaService(ITienda tiendaDao, IServicio servicioDao, IDeuda deudaDao)
        {
            _tiendaDao = tiendaDao;
            _servicioDao = servicioDao;
            _deudaDao = deudaDao;
        }

        // Método que automatiza la facturación masiva
        public void GenerarDeudasAutomaticas()
        {
            DateTime fechaActual = DateTime.Now;
            int diaActual = fechaActual.Day;

            // 1. Filtrar servicios: Solo aquellos cuyo 'DiaCorte' coincida con el día de hoy.
            var serviciosDelDia = _servicioDao.Listar()
               .Where(s => s.DiaCorte == diaActual)
               .ToList();

            // 2. Obtener el universo de tiendas del mercadito para aplicarles el cobro.
            var tiendas = _tiendaDao.Listar().ToList();

            // 3. Proceso de Facturación (Doble Bucle):
            foreach (var servicio in serviciosDelDia)
            {
                foreach (var tienda in tiendas)
                {
                    // VALIDACIÓN CRÍTICA: Llama al DAO para verificar si ya se generó este cobro
                    // para esta tienda y este servicio en el mes actual.
                    bool existe = _deudaDao.ExisteDeuda(tienda.Id, servicio.Id, fechaActual);

                    if (!existe)
                    {
                        // 4. Creación del objeto Deuda con sus reglas de negocio:
                        Deuda nuevaDeuda = new Deuda
                        {
                            MontoDeuda = servicio.Costo,        // Toma el precio base del servicio
                            FechaFacturacion = DateTime.Now,
                            FechaLimitePago = DateTime.Now.AddDays(5), // Regla: 5 días para pagar
                            Estado = "Pendiente",
                            TiendaId = tienda.Id,
                            ServicioId = servicio.Id
                        };

                        // 5. Persistencia: Se registra la deuda de forma individual en la BD.
                        _deudaDao.Registrar(nuevaDeuda);
                    }
                }
            }
        }
    }
}