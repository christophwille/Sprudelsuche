using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Geolocation;
using Windows.Services.Maps;
using Sprudelsuche.Portable;
using Sprudelsuche.Portable.Model;

namespace Sprudelsuche.WP.Services
{
    // How to authenticate your Maps app: http://msdn.microsoft.com/en-us/library/windows/apps/xaml/dn741528.aspx
    public class WpaGeocodeService : IGeocodeProxy
    {
        // http://msdn.microsoft.com/en-us/library/windows/apps/xaml/dn631249.aspx
        public async Task<List<Portable.Model.GeocodeResult>> ExecuteQuery(string query)
        {
            throw new NotImplementedException("Not yet fully implemented, testing only");

            try
            {
                // TODO: center for Austria (lat, lon), hint Austria in search string
                BasicGeoposition queryHint = new BasicGeoposition();
                queryHint.Latitude = 47.643;
                queryHint.Longitude = -122.131;
                Geopoint hintPoint = new Geopoint(queryHint);

                MapLocationFinderResult result = await MapLocationFinder.FindLocationsAsync(query, hintPoint);

                if (result.Status == MapLocationFinderStatus.Success)
                {
                    return result.Locations.Select(l => new Portable.Model.GeocodeResult()
                    {
                        Latitude = l.Point.Position.Latitude,
                        Longitude = l.Point.Position.Longitude,
                        Name = l.DisplayName,
                    }).ToList();
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
            }

            return new List<GeocodeResult>();
        }

        public Task<Portable.Model.GeocodeResult> ReverseGeocode(double latitude, double longitude)
        {
            throw new NotImplementedException();
        }
    }
}
