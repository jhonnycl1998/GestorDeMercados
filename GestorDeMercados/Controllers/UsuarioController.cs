using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using GestorDeMercados.DTO;
using GestorDeMercados.Models;
using GestorDeMercados.Repositories.Interfaces;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Authorization;

namespace GestorDeMercados.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    
    public class UsuarioController : Controller
    {
        private readonly IUsuario _usuarioDao;

        private readonly IConfiguration _config;

        // Inyección de dependencias para la gestión de credenciales y perfiles
        public UsuarioController(IUsuario usuarioDao, IConfiguration config)
        {
            _usuarioDao = usuarioDao;
            _config = config;
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
        [Authorize(Roles = "Admin")]
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
        [Authorize(Roles = "Admin")]

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
        [Authorize(Roles = "Admin")]

        public ActionResult DeleteUsuario(int id)
        {
            _usuarioDao.Eliminar(id);
            return Ok();
        }

        // Lógica de Autenticación (Login):
        // Valida las credenciales comparando el hash de la base de datos con la clave ingresada.
        [HttpPost("login")]
        [AllowAnonymous]
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

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.id.ToString()),
                new Claim(ClaimTypes.Name, user.usuario),
                new Claim(ClaimTypes.Role, user.Rol!.Nombre)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]!));
            var token = new JwtSecurityToken(

                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(int.Parse(_config["Jwt:ExpireMinutes"]!)),
                signingCredentials: new SigningCredentials(key, SecurityAlgorithms.HmacSha256));

            // Retorna los datos esenciales para la sesión (Usuario y Rol)
            return Ok(new
            {
                duenoId = user.Dueno?.Id,
                mensaje = "Login exitoso",
                usuario = user.usuario,
                rol = user.Rol.Nombre,
                token = new JwtSecurityTokenHandler().WriteToken(token)
            });
        }
        [HttpPut("cambiarContrasena")]
        public IActionResult CambiarContrasena([FromBody] CambiarcContrasenaDTO dto)
        {
            var id = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var user = _usuarioDao.Buscar(id);

            if (user == null)
                return NotFound();

            var hasher = new PasswordHasher<Usuario>();

            if (hasher.VerifyHashedPassword(user, user.contrasenaHash, dto.contrasenaActual)
                == PasswordVerificationResult.Failed)
                return BadRequest(new { mensaje = "La contraseña actual es incorrecta" });

            user.contrasenaHash = hasher.HashPassword(user, dto.contrasenaNueva);
            _usuarioDao.Actualizar(user);

            return Ok(new { mensaje = "Contraseña actualizada" });
        }
    }
}