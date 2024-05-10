using System.ComponentModel.DataAnnotations;

namespace PruebaAtlantidaBE.Models
{
    public class TipoMovimientoCLS
    {
        [Key]
        public int Id;

        [Required]
        public string Nombre;
    }
}
