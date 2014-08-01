using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sprudelsuche.Portable.Model;

namespace Sprudelsuche.WP.Models
{
    public class Favorite
    {
        public string LocationName { get; set; }
        public FuelTypeEnum FuelType { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
    }
}
