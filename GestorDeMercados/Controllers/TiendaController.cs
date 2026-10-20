using Microsoft.AspNetCore.Mvc;
using GestorDeMercados.Models;
using GestorDeMercados.Repositories.Interfaces;
using Microsoft.AspNetCore.Authorization;
using GestorDeMercados.Data;

namespace GestorDeMercados.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
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
        [Authorize(Roles ="Admin")]
        public ActionResult SaveTienda(Tienda tienda)
        {

            // Si el objeto enviado por Postman no tiene DuenoId (es 0 o null)
            if (!tienda.TieneDueno)
            {
                tienda.DuenoId = _duenoDao.Listar().First(d => d.ruc == DbSeeder.RucAsociacion).Id;
            }
            else if (tienda.DuenoId == null || tienda.DuenoId == 0)
            {
                return BadRequest(new { mensaje = "Debe indicar el dueño de la tienda." });
            }

            

            _tiendaDao.Registrar(tienda);
            return Created("", tienda);
        }

        [HttpPut("updateTienda")]
        [Authorize(Roles = "Admin")]
        
        public ActionResult UpdateTienda(Tienda tienda)
        {
            // Validamos el estado del dueño antes de actualizar
            try
            {
                if (!tienda.TieneDueno)
                {
                    tienda.DuenoId = _duenoDao.Listar().First(d => d.ruc == DbSeeder.RucAsociacion).Id;
                }
                else if (tienda.DuenoId == null || tienda.DuenoId == 0)
                {
                    return BadRequest(new { mensaje = "Debe indicar el dueño de la tienda." });
                }

                _tiendaDao.Actualizar(tienda);
                return Ok(tienda);
            }
            catch (Exception ex)
            {

                return BadRequest(new {mensaje = ex.Message});
            }
            
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public ActionResult DeleteTienda(int id)
        {
            _tiendaDao.Eliminar(id);
            return Ok();
        }
    }
}