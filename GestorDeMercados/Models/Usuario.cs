using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace GestorDeMercados.Models
{
    public class Usuario
    {
        [Key]
        [Display(Name = "IdUsuario")]
        public int id { get; set; }

        [Display(Name = "Usuario")]
        public string usuario { get; set; }

        [Display(Name = "Contraseña")]
        public string contrasenaHash { get; set; }

        public int RolId { get; set; }
        public Rol? Rol { get; set; }

        [JsonIgnore]
        public Dueno? Dueno { get; set; }
    }
}

