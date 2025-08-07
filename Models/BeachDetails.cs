using System;

namespace SurfingPointServer.Models
{
    public class BeachDetails
    {
        public int BeachID { get; set; }
        public string Name { get; set; }
        public string Location { get; set; }
        public decimal WaveHeight { get; set; }
        public float WindSpeed { get; set; }
        public DateTime ForecastDate { get; set; }
       
    }
}
