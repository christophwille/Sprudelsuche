using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sprudelsuche.Portable.Model;

namespace Sprudelsuche.WP.Models
{
    public class MainViewModelState
    {
        public string SearchText { get; set; }
        public FuelTypeEnum FuelType { get; set; }
        public List<GeocodeResult> Results { get; set; }
    }
}
