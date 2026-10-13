using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace GestorDeMercados.Models
{
    public class Tienda
    {
        [Key]
        [Display(Name = "IdTienda")]
        public int Id { get; set; }

        [Display(Name = "ValidarDueño")]
        public bool TieneDueno { get; set; }

        [Display(Name = "Alquiler")]
        public bool alquiler { get; set; }

        [Display(Name = "CostoAlquiler")]
        public double costoAlquiler { get; set; }

        public int? DuenoId { get; set; }
        [JsonIgnore]
        public Dueno? Dueno { get; set; }

        [JsonIgnore]
        public List<Deuda> Deudas { get; set; } = new();

    }
}
