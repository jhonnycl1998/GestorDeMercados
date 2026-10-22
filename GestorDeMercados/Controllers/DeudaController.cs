using Microsoft.AspNetCore.Mvc;
using GestorDeMercados.Models;
using GestorDeMercados.Repositories.Interfaces;
using Microsoft.AspNetCore.Authorization;

namespace GestorDeMercados.Controllers
{
    // Define que esta clase es un controlador de API y establece la ruta base (api/Deuda)
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class DeudaController : Controller
    {
        private readonly IDeuda _deudaDao;

        // Inyección de dependencias para desacoplar la lógica de acceso a datos de la API
        public DeudaController(IDeuda deudaDao)
        {
            _deudaDao = deudaDao;
        }

        // Obtiene el listado completo de deudas para reportes generales
        [HttpGet("getDeudas")]
        public ActionResult GetDeudas()
        {
            return Ok(_deudaDao.Listar());
        }

        // Busca una deuda específica por su ID para verificar su estado actual
        [HttpGet("{id}")]
        public ActionResult GetDeuda(int id)
        {
            var deuda = _deudaDao.Buscar(id);

            if (deuda == null)
                return NotFound(); // Retorna 404 si la deuda no existe en la BD

            return Ok(deuda);
        }

        // Registra una nueva deuda (ej. Luz, Agua) asociada a una tienda y servicio
        [HttpPost("saveDeuda")]
        public ActionResult SaveDeuda(Deuda deuda)
        {
            _deudaDao.Registrar(deuda);
            return Created("", deuda); // Retorna 201 confirmando la creación del recurso
        }

        // Permite modificar datos de una deuda existente (ej. cambiar monto o estado)
        [HttpPut("updateDeuda")]
        public ActionResult UpdateDeuda(Deuda deuda)
        {
            _deudaDao.Actualizar(deuda);
            return Ok(deuda);
        }

        // Elimina lógicamente o físicamente un registro de deuda por su ID
        [HttpDelete("{id}")]
        public ActionResult DeleteDeuda(int id)
        {
            _deudaDao.Eliminar(id);
            return Ok();
        }

        // Filtra deudas pendientes de un dueño específico para proceder con el pago
        [HttpGet("dueno/{duenoId}")]
        public IActionResult ObtenerPendientesPorDueno(int duenoId)
        {
            var deudas = _deudaDao.ObtenerPendientesPorDueno(duenoId);

            // Validación de existencia: Si no hay deudas, informa al usuario
            if (deudas == null || !deudas.Any())
                return NotFound(new { mensaje = "No hay deudas pendientes para ese dueño" });

            return Ok(deudas);
        }
    }
}