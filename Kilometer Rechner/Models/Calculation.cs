using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Kilometer_Rechner.Models
{
    [Table (name: "Calculations", Schema = "dbo")]
    public class Calculation
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int BasePlz { get; set; }
        public int IdPlz { get; set; }
        public DateTime CalcDate { get; set; }
        public double AirLineKm { get; set; }
        public double RouteLineKm { get; set; }
    }
}