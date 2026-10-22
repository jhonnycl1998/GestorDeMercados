using GestorDeMercados.Models;
using GestorDeMercados.Repositories.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace GestorDeMercados.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class PagoController : Controller
    {
        private readonly IPago _pagoDAO;

        // Inyección de la interfaz IPago para manejar la lógica de transacciones financieras
        public PagoController(IPago pagoDAO)
        {
            _pagoDAO = pagoDAO;
        }

        // Obtiene el historial completo de pagos realizados en el mercadito
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public IActionResult Listar()
        {
            var lista = _pagoDAO.Listar();
            return Ok(lista);
        }

        // Recupera la información de un pago específico mediante su ID
        [HttpGet("{id}")]
        public IActionResult ObtenerPorId(int id)
        {
            var pago = _pagoDAO.ObtenerPorId(id);

            if (pago == null)
                return NotFound(new { mensaje = "Pago no encontrado" });

            return Ok(pago);
        }

        // Permite consultar si una deuda específica ya cuenta con un pago registrado
        [HttpGet("deuda/{deudaId}")]
        public IActionResult ObtenerPorDeudaId(int deudaId)
        {
            var pago = _pagoDAO.ObtenerPorDeudaId(deudaId);

            if (pago == null)
                return NotFound(new { mensaje = "No existe pago para esa deuda" });

            return Ok(pago);
        }

        
        // Incluye un bloque try-catch para capturar validaciones de negocio (ej. montos que no coinciden)
        [HttpPost]
        public IActionResult Registrar([FromBody] Pago pago)
        {
            try
            {
                var nuevoPago = _pagoDAO.RegistrarPago(pago);
                return Ok(new
                {
                    mensaje = "Pago registrado correctamente",
                    pago = nuevoPago
                });
            }
            catch (Exception ex)
            {
                // Retorna el error de validación (como el que vimos de "el monto debe ser igual")
                return BadRequest(new { mensaje = ex.Message });
            }
        }

        // Elimina un registro de pago y libera la deuda asociada (si la lógica del DAO lo permite)
        [HttpDelete("{id}")]
        public IActionResult Eliminar(int id)
        {
            var eliminado = _pagoDAO.Eliminar(id);

            if (!eliminado)
                return NotFound(new { mensaje = "Pago no encontrado" });

            return Ok(new { mensaje = "Pago eliminado correctamente" });
        }

        
        // Este método construye una vista web dinámica usando los datos relacionados del Dueño, Tienda y Deuda.
        [HttpGet("voucher-html/{id}")]
        public IActionResult VoucherHtml(int id)
        {
            var pago = _pagoDAO.ObtenerPorId(id);

            if (pago == null)
                return NotFound("Pago no encontrado");

            // Se utiliza interpolación de cadenas para inyectar los datos en la plantilla HTML
            var html = $@"
    <html>
    <head>
        <meta charset='UTF-8'>
        <title>Voucher de Pago</title>
        <style>
            body {{ font-family: Arial, sans-serif; margin: 40px; }}
            .voucher {{ border: 1px solid #ccc; padding: 20px; max-width: 500px; box-shadow: 2px 2px 10px #eee; }}
            h1 {{ text-align: center; color: #2c3e50; }}
            p {{ margin: 8px 0; border-bottom: 1px solid #f9f9f9; }}
            strong {{ color: #34495e; }}
        </style>
    </head>
    <body>
        <div class='voucher'>
            <h1>Voucher de Pago</h1>
            <p><strong>ID Pago:</strong> {pago.Id}</p>
            <p><strong>Dueño:</strong> {pago.Deuda?.Tienda?.Dueno?.nombre} {pago.Deuda?.Tienda?.Dueno?.apellido}</p>
            <p><strong>RUC:</strong> {pago.Deuda?.Tienda?.Dueno?.ruc}</p>
            <p><strong>Fecha de Pago:</strong> {pago.FechaPago}</p>
            <p><strong>Monto Pagado:</strong> S/ {pago.MontoPago}</p>
            <p><strong>Servicio:</strong> {pago.Deuda?.Servicio?.Nombre}</p>
            <p><strong>Estado de Deuda:</strong> {pago.Deuda?.Estado}</p>
        </div>
    </body>
    </html>";

            return Content(html, "text/html");
        }

        // Pagos del dueño que inició sesión (el id sale del token JWT)
        [HttpGet("mis-pagos")]
        public IActionResult MisPagos()
        {
            var usuarioId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var pagos = _pagoDAO.ListarPorUsuario(usuarioId).Select(p => new
            {
                p.Id,
                p.FechaPago,
                p.MontoPago,
                p.DeudaId,
                TiendaId = p.Deuda?.TiendaId,
                ServicioNombre = p.Deuda?.Servicio?.Nombre
            });
            return Ok(pagos);
        }

        
        // Filtra los registros de la base de datos cuya fecha coincida con la actual.
        [HttpGet("reporte-hoy")]
        public IActionResult ReporteHoy()
        {
            var pagos = _pagoDAO.ObtenerPagosDeHoy();

            if (pagos == null || !pagos.Any())
                return NotFound(new { mensaje = "No hay pagos registrados hoy" });

            return Ok(pagos);
        }
    }
}