using Microsoft.AspNetCore.Mvc;
using GestorDeMercados.Models;
using GestorDeMercados.Repositories.Interfaces;

namespace GestorDeMercados.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TiendaController : Controller
    {
        private readonly ITienda _tiendaDao;
        private readonly IDueno _duenoDao;

        // El constructor recibe ambas interfaces gracias a la Inyección de Dependencias
        public TiendaController(ITienda tiendaDao, IDueno duenoDao)
        {
            _tiendaDao = tiendaDao;
            _duenoDao = duenoDao;
        }

        [HttpGet("getTiendas")]
        public ActionResult GetTiendas()
        {
            return Ok(_tiendaDao.Listar());
        }

        [HttpGet("{id}")]
        public ActionResult GetTienda(int id)
        {
            var tienda = _tiendaDao.Buscar(id);

            if (tienda == null)
                return NotFound();

            return Ok(tienda);
        }

        [HttpPost("saveTienda")]
        public ActionResult SaveTienda(Tienda tienda)
        {

            // Si el objeto enviado por Postman no tiene DuenoId (es 0 o null)
            if (tienda.DuenoId == 0 || tienda.DuenoId == null)
            {
                // Obtenemos la lista de dueños y tomamos el primero disponible
                var duenoAsignado = _duenoDao.Listar().FirstOrDefault();

                if (duenoAsignado != null)
                {
                    tienda.DuenoId = duenoAsignado.Id;
                    tienda.TieneDueno = true;
                }
                else
                {
                    // Si la tabla Dueños está vacía, no se puede asociar nada
                    tienda.TieneDueno = false;
                }
            }
            else
            {
                // Si el usuario ya mandó un ID de dueño específico
                tienda.TieneDueno = true;
            }

            _tiendaDao.Registrar(tienda);
            return Created("", tienda);
        }

        [HttpPut("updateTienda")]
        public ActionResult UpdateTienda(Tienda tienda)
        {
            // Validamos el estado del dueño antes de actualizar
            tienda.TieneDueno = tienda.DuenoId > 0;

            _tiendaDao.Actualizar(tienda);
            return Ok(tienda);
        }

        [HttpDelete("{id}")]
        public ActionResult DeleteTienda(int id)
        {
            _tiendaDao.Eliminar(id);
            return Ok();
        }
    }
}