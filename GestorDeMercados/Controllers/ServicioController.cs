using Microsoft.AspNetCore.Mvc;
using GestorDeMercados.Models;
using GestorDeMercados.Repositories.Interfaces;

namespace GestorDeMercados.Controllers
{
    // Define el controlador para la gestión de servicios maestros
    [ApiController]
    [Route("api/[controller]")]
    public class ServicioController : Controller
    {
        private readonly IServicio _servicioDao;

        // Inyección de dependencias para la lógica de negocio de servicios
        public ServicioController(IServicio servicioDao)
        {
            _servicioDao = servicioDao;
        }

        // Obtiene el catálogo de servicios (Luz, Agua, Mantenimiento, etc.)
        [HttpGet("getServicios")]
        public ActionResult GetServicios()
        {
            return Ok(_servicioDao.Listar());
        }

        // Busca un servicio específico por su ID
        [HttpGet("{id}")]
        public ActionResult GetServicio(int id)
        {
            var servicio = _servicioDao.Buscar(id);

            if (servicio == null)
                return NotFound(new { mensaje = "Servicio no encontrado" });

            return Ok(servicio);
        }

        // Registra un nuevo tipo de servicio en el sistema.
        // Es fundamental para cumplir con la integridad referencial al crear Deudas.
        [HttpPost("saveServicio")]
        public ActionResult SaveServicio(Servicio servicio)
        {
            _servicioDao.Registrar(servicio);
            return Created("", servicio); // Retorna 201 Created confirmando el éxito
        }

        // Actualiza la descripción o el nombre de un servicio existente
        [HttpPut("updateServicio")]
        public ActionResult UpdateServicio(Servicio servicio)
        {
            _servicioDao.Actualizar(servicio);
            return Ok(servicio);
        }

        // Elimina un servicio del catálogo por su ID
        [HttpDelete("{id}")]
        public ActionResult DeleteServicio(int id)
        {
            _servicioDao.Eliminar(id);
            return Ok(new { mensaje = "Servicio eliminado correctamente" });
        }
    }
}