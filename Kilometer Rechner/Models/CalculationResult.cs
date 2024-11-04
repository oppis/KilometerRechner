namespace Kilometer_Rechner.Models
{
    public class CalculationResult
    {
        public DateTime CalcDate { get; set; }
        public string StartPlz {  get; set; }
        public string StartOrt { get; set; }
        public string EndPlz { get; set; }
        public string EndOrt { get; set; }
        public double AirLineKm { get; set; }
        public double RouteLineKm { get; set; }
        public double FaktorCalc {  get; set; }
    }
}