using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace GestorDeMercados.Models
{
    public class Rol
    {
        [Key]
        [Display(Name = "IdRol")]
        public int Id { get; set; }

        [Display(Name = "Nombre")]
        public string Nombre { get; set; }

        [JsonIgnore]
        public List<Usuario> Usuarios { get; set; } = new();
    }
}
