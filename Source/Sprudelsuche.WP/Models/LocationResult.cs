using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Geolocation;

namespace Sprudelsuche.WP.Models
{
    public class LocationResult
    {
        public LocationResult(Geocoordinate pos)
        {
            Coordinate = pos;
        }

        public LocationResult(string errMsg)
        {
            ErrorMessage = errMsg;
        }

        public Geocoordinate Coordinate { get; set; }

        public bool Succeeded
        {
            get { return null != Coordinate; }
        }

        public string ErrorMessage { get; set; }
    }
}
