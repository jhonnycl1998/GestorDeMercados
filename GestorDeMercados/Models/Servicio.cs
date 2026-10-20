using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace GestorDeMercados.Models
{
    public class Servicio
    {
        [Key]
        [Display(Name = "ID Servicio")]
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        [Display(Name = "Nombre del Servicio")]
        public string Nombre { get; set; }

        [Required]
        [Display(Name = "Costo del Servicio")]
        public decimal Costo { get; set; }

        [Required]
        [Display(Name = "Dia de Corte")]
        public int DiaCorte { get; set; }

        // Un servicio puede tener muchas deudas
        [JsonIgnore]
        public List<Deuda> Deudas { get; set; } = new();
    }
}
