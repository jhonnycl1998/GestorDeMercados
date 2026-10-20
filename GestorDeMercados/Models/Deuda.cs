using System.ComponentModel.DataAnnotations;

namespace GestorDeMercados.Models
{
    public class Deuda
    {
        [Key]
        [Display(Name = "ID Deuda")]
        public int Id { get; set; }

        [Required]
        [Display(Name = "Monto de la Deuda")]
        public decimal MontoDeuda { get; set; }

        [Required]
        [Display(Name = "Fecha de Facturación")]
        public DateTime FechaFacturacion { get; set; }

        [Required]
        [Display(Name = "Fecha Límite de Pago")]
        public DateTime FechaLimitePago { get; set; }

        [Required]
        [StringLength(50)]
        [Display(Name = "Estado")]
        public string Estado { get; set; }

        [Display(Name = "Tienda")]
        public int TiendaId { get; set; }
        public Tienda? Tienda { get; set; }

        [Display(Name = "Servicio")]
        public int ServicioId { get; set; }
        public Servicio? Servicio { get; set; }

        public Pago? Pago { get; set; }

    }
}
