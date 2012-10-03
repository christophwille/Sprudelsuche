using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using Sprudelsuche.Portable.Model;

namespace Sprudelsuche.Model
{
    public class AddSprudelSuchePageState
    {
        public string SearchText { get; set; }
        public List<GeocodeResult> GeocodeResults { get; set; }
        public string SelectedGeocodeResultUniqueId { get; set; }
    }
}
