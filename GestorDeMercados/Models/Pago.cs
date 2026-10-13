using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace GestorDeMercados.Models
{
    public class Pago
    {
        [Key]
        [Display(Name = "IDPago")]
        public int Id { get; set; }

        [Required]
        [Display(Name = "Fecha de Pago")]
        public DateTime FechaPago { get; set; }

        [Required]
        [Display(Name = "Monto Pagado")]
        [Column(TypeName = "decimal(18,2)")]
        public decimal MontoPago { get; set; }

        // FK única para relación 1 a 1 con Deuda
        [Display(Name = "Deuda")]
        public int DeudaId { get; set; }

        [JsonIgnore]
        public virtual Deuda? Deuda { get; set; }
    }
}
