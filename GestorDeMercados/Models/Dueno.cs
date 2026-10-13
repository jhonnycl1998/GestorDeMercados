using System.ComponentModel.DataAnnotations;

namespace GestorDeMercados.Models
{
    public class Dueno
    {
        [Key]
        [Display(Name = "Id_Dueño")]
        public int Id { get; set; }

        [Display(Name = "nombre")]
        public string nombre { get; set; }

        [Display(Name = "apellido")]
        public string apellido { get; set; }

        [Display(Name = "Ruc")]
        public string ruc { get; set; }

        [Display(Name = "Fecha_Nacimiento")]
        public DateTime fechaNacimiento { get; set; }

        [Display(Name = "Direccion")]
        public string direccion { get; set; }

        public int UsuarioId { get; set; }
        public Usuario? Usuario { get; set; }


        public List<Tienda> Tiendas { get; set; } = new();




    }
}
