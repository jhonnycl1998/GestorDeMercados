using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using GestorDeMercados.DTO;
using GestorDeMercados.Models;
using GestorDeMercados.Repositories.Interfaces;

namespace GestorDeMercados.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsuarioController : Controller
    {
        private readonly IUsuario _usuarioDao;

        // Inyección de dependencias para la gestión de credenciales y perfiles
        public UsuarioController(IUsuario usuarioDao)
        {
            _usuarioDao = usuarioDao;
        }

        // Obtiene la lista de todos los usuarios registrados
        [HttpGet("getUsuarios")]
        public ActionResult GetUsuarios()
        {
            return Ok(_usuarioDao.Listar());
        }

        // Busca un usuario por su ID para gestionar su perfil
        [HttpGet("{id}")]
        public ActionResult GetUsuario(int id)
        {
            var usuario = _usuarioDao.Buscar(id);

            if (usuario == null)
                return NotFound();

            return Ok(usuario);
        }

        // RF-01: Registro de nuevo usuario con seguridad integrada.
        // Implementa PasswordHasher para encriptar la contraseña antes de guardarla en la BD.
        [HttpPost("saveUsuario")]
        public ActionResult SaveUsuario(Usuario usuario)
        {
            var hasher = new PasswordHasher<Usuario>();

            // Protege la integridad del dato: se guarda un Hash, nunca la clave real
            usuario.contrasenaHash = hasher.HashPassword(usuario, usuario.contrasenaHash);

            _usuarioDao.Registrar(usuario);
            return Created("", usuario);
        }

        // Actualiza la información del usuario (nombre, correo, rol, etc.)
        [HttpPut("updateUsuario")]
        public ActionResult UpdateUsuario(Usuario usuario)
        {
            var existente = _usuarioDao.Buscar(usuario.id);

            if (existente == null)
                return NotFound();

            var hasher = new PasswordHasher<Usuario>();

            // Solo si cambian contraseña
            if (!string.IsNullOrEmpty(usuario.contrasenaHash))
            {
                existente.contrasenaHash = hasher.HashPassword(existente, usuario.contrasenaHash);
            }

            existente.usuario = usuario.usuario;
            existente.RolId = usuario.RolId;

            _usuarioDao.Actualizar(existente);

            return Ok(existente);
        }

        // Elimina una cuenta de usuario del sistema
        [HttpDelete("{id}")]
        public ActionResult DeleteUsuario(int id)
        {
            _usuarioDao.Eliminar(id);
            return Ok();
        }

        // Lógica de Autenticación (Login):
        // Valida las credenciales comparando el hash de la base de datos con la clave ingresada.
        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginDTO dto)
        {
            var user = _usuarioDao.ObtenerPorUsuario(dto.usuario);

            // Validación de existencia
            if (user == null)
                return Unauthorized(new { mensaje = "Usuario no existe" });

            var hasher = new PasswordHasher<Usuario>();

            // Verifica si el texto ingresado coincide con el hash almacenado
            var resultado = hasher.VerifyHashedPassword(
                user,
                user.contrasenaHash,
                dto.contrasena
            );

            if (resultado == PasswordVerificationResult.Failed)
                return Unauthorized(new { mensaje = "Contraseña incorrecta" });

            // Retorna los datos esenciales para la sesión (Usuario y Rol)
            return Ok(new
            {
                mensaje = "Login exitoso",
                usuario = user.usuario,
                rol = user.RolId
            });
        }
    }
}