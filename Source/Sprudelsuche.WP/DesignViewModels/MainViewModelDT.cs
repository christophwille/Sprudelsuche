using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sprudelsuche.Portable.Model;
using Sprudelsuche.WP.Models;

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

            Favorites = new List<Favorite>()
            {
                new Favorite()
                {
                    LocationName = "Bad Ischl"
                }
            };
        }

        public string VersionText { get; set; }
        public List<GeocodeResult> Results { get; set; }
        public List<Favorite> Favorites { get; set; }
    }
}
