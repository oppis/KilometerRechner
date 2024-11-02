using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

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
        public float Lon { get; set; }
        public float Lat {  get; set; }
    }
}