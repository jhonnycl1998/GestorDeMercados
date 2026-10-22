using GestorDeMercados.Models;
using GestorDeMercados.Repositories.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace GestorDeMercados.Controllers
{
    // Indica que es un controlador de API y define el acceso mediante api/Dueno
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class DuenoController : Controller
    {
        private readonly IDueno _duenoDao;
        private readonly IRol _rolDao;

        // Uso de Inyección de Dependencias para interactuar con la capa de datos de Dueños
        public DuenoController(IDueno duenoDao, IRol rolDao)
        {
            _duenoDao = duenoDao;
            _rolDao = rolDao;
        }

        // Obtiene la lista de todos los dueños registrados en el sistema
        [HttpGet("getDuenos")]
        public ActionResult GetDuenos()
        {
            return Ok(_duenoDao.Listar());
        }

        // Busca un dueño por su ID único; esencial para vincularlo con tiendas o deudas
        [HttpGet("{id}")]
        public ActionResult GetDueno(int id)
        {
            var dueno = _duenoDao.Buscar(id);

            if (dueno == null)
                return NotFound(new { mensaje = "El dueño solicitado no existe." });

            return Ok(dueno);
        }

        // Registra un nuevo dueño en el sistema. 
        // Este registro es el que permite luego la "Asociación Automática" en las tiendas.
        [HttpPost("saveDueno")]
        [Authorize(Roles = "Admin")]
        public ActionResult SaveDueno(Dueno dueno)
        {
            if (dueno.Usuario == null)
                return BadRequest(new { mensaje = "El dueño debe incluir un usuario." });
            
                var hasher = new PasswordHasher<Usuario>();

                dueno.Usuario.contrasenaHash = hasher.HashPassword(
                    dueno.Usuario,
                    dueno.Usuario.contrasenaHash
                    
                );
                dueno.Usuario.RolId = _rolDao.Listar().First(r => r.Nombre == "User").Id;
            

            _duenoDao.Registrar(dueno);

            return Created("", dueno); // Retorna 201 Created con los datos del nuevo dueño
        }

        // Actualiza la información personal o de contacto del dueño
        [HttpPut("updateDueno")]
        [Authorize(Roles = "Admin")]
        public ActionResult UpdateDueno(Dueno dueno)
        {
            var existente = _duenoDao.Buscar(dueno.Id);

            if (existente == null)
                return NotFound(new { mensaje = "El dueño no existe." });

            existente.nombre = dueno.nombre;
            existente.apellido = dueno.apellido;
            existente.ruc = dueno.ruc;
            existente.fechaNacimiento = dueno.fechaNacimiento;
            existente.direccion = dueno.direccion;

            _duenoDao.Actualizar(existente);
            return Ok(existente);
        }

        // Elimina el registro de un dueño del sistema por su ID
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public ActionResult DeleteDueno(int id)
        {
            try
            {
                _duenoDao.Eliminar(id);
                return Ok(new { mensaje = "Dueño eliminado correctamente" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { mensaje = ex.Message });                
            }
            
        }
    }
}