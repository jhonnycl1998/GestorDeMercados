using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using GestorDeMercados.Models;
using GestorDeMercados.Repositories.Interfaces;

namespace GestorDeMercados.Controllers
{
    // Indica que es un controlador de API y define el acceso mediante api/Dueno
    [ApiController]
    [Route("api/[controller]")]
    public class DuenoController : Controller
    {
        private readonly IDueno _duenoDao;

        // Uso de Inyección de Dependencias para interactuar con la capa de datos de Dueños
        public DuenoController(IDueno duenoDao)
        {
            _duenoDao = duenoDao;
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

        // RNF-01: Registra un nuevo dueño en el sistema. 
        // Este registro es el que permite luego la "Asociación Automática" en las tiendas.
        [HttpPost("saveDueno")]
        public ActionResult SaveDueno(Dueno dueno)
        {
            if (dueno.Usuario != null)
            {
                var hasher = new PasswordHasher<Usuario>();

                dueno.Usuario.contrasenaHash = hasher.HashPassword(
                    dueno.Usuario,
                    dueno.Usuario.contrasenaHash
                );
            }

            _duenoDao.Registrar(dueno);

            return Created("", dueno); // Retorna 201 Created con los datos del nuevo dueño
        }

        // Actualiza la información personal o de contacto del dueño
        [HttpPut("updateDueno")]
        public ActionResult UpdateDueno(Dueno dueno)
        {
            _duenoDao.Actualizar(dueno);
            return Ok(dueno);
        }

        // Elimina el registro de un dueño del sistema por su ID
        [HttpDelete("{id}")]
        public ActionResult DeleteDueno(int id)
        {
            _duenoDao.Eliminar(id);
            return Ok(new { mensaje = "Dueño eliminado correctamente" });
        }
    }
}