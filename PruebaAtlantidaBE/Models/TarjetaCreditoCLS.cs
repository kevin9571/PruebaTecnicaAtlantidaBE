using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PruebaAtlantidaBE.Models
{
    public class TarjetaCreditoCLS
    {
        [Key]
        [Column("Id")]
        public int Id { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "Longitud maxima 100")]
        [Column("Nombre")]
        public string Nombre { get; set; }

        [Required]
        [StringLength(16, ErrorMessage = "Longitud maxima 16")]
        [Column("Numero")]
        public string Numero { get; set; }

        [Column("SaldoActual")]
        public double SaldoActual { get; set; }

        [Required]
        [Column("SaldoDisponible")]
        public double SaldoDisponible { get; set; }

        [Required]
        [Column("Limite")]
        public double Limite { get; set; }

        [Required]
        [Range(0, 1, ErrorMessage = "Porcentaje de interes Fuera de rango")]
        [Column("PorcentajeInteresConfigurable")]
        public double PorcentajeInteresConfigurable { get; set; }

        [Required]
        [Range(0, 1, ErrorMessage = "Porcentaje de Saldo minimo Fuera de rango")]
        [Column("PorcentajeConfigurableSaldoMinimo")]
        public double PorcentajeConfigurableSaldoMinimo { get; set; }
    }
}
