using System;

namespace Sprudelsuche.Portable.Model
{
    public class GeocodeResult
    {
        public GeocodeResult()
        {
            UniqueId = Guid.NewGuid().ToString();
        }

        public string UniqueId { get; set; }

        public string Name { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
    }
}
