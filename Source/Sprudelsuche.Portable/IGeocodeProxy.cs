using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sprudelsuche.Portable.Model;

namespace Sprudelsuche.Portable
{
    public interface IGeocodeProxy
    {
        Task<List<GeocodeResult>> ExecuteQuery(string query);
        Task<GeocodeResult> ReverseGeocode(double latitude, double longitude);
    }
}
