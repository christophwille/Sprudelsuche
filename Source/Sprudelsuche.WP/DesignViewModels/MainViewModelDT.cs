using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sprudelsuche.Portable.Model;

namespace Sprudelsuche.WP.DesignViewModels
{
    public class MainViewModelDT
    {
        public MainViewModelDT()
        {
            VersionText = "15.0.0.0";

            Results = new List<GeocodeResult>()
            {
                new GeocodeResult()
                {
                    Name = "Leoben"
                },
                new GeocodeResult()
                {
                    Name = "Graz"
                }
            };
        }

        public string VersionText { get; set; }
        public List<GeocodeResult> Results { get; set; }
    }
}
