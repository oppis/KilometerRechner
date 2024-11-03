using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Threading.Tasks.Dataflow;

namespace Kilometer_Rechner.Models
{
    [Table(name: "Cities", Schema = "dbo")]
    public class CityModel
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [MaxLength(10)]
        public string PLZ { get; set; }
        [MaxLength(100)]
        public string Ort { get; set; }

        public double Latitude { get; set; }
        public double Longitude {  get; set; }
    }
}