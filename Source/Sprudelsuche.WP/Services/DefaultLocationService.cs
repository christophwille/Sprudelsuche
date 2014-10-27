using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Windows.Devices.Geolocation;
using Sprudelsuche.WP.Models;

namespace Sprudelsuche.WP.Services
{
    // http://msdn.microsoft.com/en-us/library/windows/apps/hh465148.aspx
    public class DefaultLocationService : ILocationService
    {
        public async Task<LocationResult> GetCurrentPositionAsync()
        {
            try
            {
                var geolocator = new Geolocator()
                {
                    DesiredAccuracyInMeters = 500
                };

                Geoposition pos = await geolocator.GetGeopositionAsync(
                    maximumAge: TimeSpan.FromMinutes(5),
                    timeout: TimeSpan.FromSeconds(10)
                    );

                return new LocationResult(pos.Coordinate);
            }
            catch (UnauthorizedAccessException)
            {
                return new LocationResult("Die aktuelle Position konnte nicht ermittelt werden, Zugriff wurde verweigert.");
            }
            catch (Exception)
            {
            }

            return new LocationResult("Aktuelle Position konnte nicht ermittelt werden");
        }
    }
}
