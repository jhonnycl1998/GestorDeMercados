using Microsoft.AspNetCore.Mvc;
using GestorDeMercados.Models;
using GestorDeMercados.Repositories.Interfaces;
using Microsoft.AspNetCore.Authorization;

namespace GestorDeMercados.Controllers
{
    // Define que es un controlador de API y establece la ruta base: api/Rol
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class RolController : Controller
    {
        private readonly IRol _rolDao;

        // Uso de Inyección de Dependencias para desacoplar la lógica de roles del acceso a datos
        public RolController(IRol rolDao)
        {
            _rolDao = rolDao;
        }

        // Obtiene el catálogo completo de roles disponibles en el sistema
        [HttpGet("getRoles")]
        public ActionResult GetRoles()
        {
            return Ok(_rolDao.Listar());
        }

        // Busca un rol específico por su ID único
        [HttpGet("{id}")]
        public ActionResult GetRol(int id)
        {
            var rol = _rolDao.Buscar(id);

            if (rol == null)
                return NotFound(new { mensaje = "El rol no existe." });

            return Ok(rol);
        }

        // RF-01: Registra un nuevo tipo de rol (ej. 'Administrador', 'Dueño')
        // Este es el primer paso antes de registrar cualquier usuario en el sistema.
        [HttpPost("saveRol")]
        public ActionResult SaveRol(Rol rol)
        {
            _rolDao.Registrar(rol);
            return Created("", rol); // Retorna 201 Created confirmando el éxito del registro
        }

        // Permite modificar la descripción o nombre de un rol existente
        [HttpPut("updateRol")]
        public ActionResult UpdateRol(Rol rol)
        {
            _rolDao.Actualizar(rol);
            return Ok(rol);
        }

        // Elimina un rol del sistema mediante su identificador
        [HttpDelete("{id}")]
        public ActionResult DeleteRol(int id)
        {
            _rolDao.Eliminar(id);
            return Ok(new { mensaje = "Rol eliminado correctamente." });
        }
    }
}