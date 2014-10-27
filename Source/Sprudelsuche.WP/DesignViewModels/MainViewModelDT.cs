using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Sprudelsuche.Portable.Model;
using Sprudelsuche.WP.Models;
using Sprudelsuche.WP.ViewModels;

namespace Sprudelsuche.WP.DesignViewModels
{
    public class MainViewModelDT : IMainViewModelBindings
    {
        public MainViewModelDT()
        {
            VersionText = "15.0.0.0";

            Results = new ObservableCollection<GeocodeResult>(new List<GeocodeResult>()
            {
                new GeocodeResult()
                {
                    Name = "Leoben"
                },
                new GeocodeResult()
                {
                    Name = "Graz"
                }
            });

            Favorites = new ObservableCollection<Favorite>(new List<Favorite>()
            {
                new Favorite()
                {
                    LocationName = "Bad Ischl",
                    FuelType = FuelTypeEnum.Diesel,
                }
            });

            SearchByLocation = true;
            DieselSelected = true;
        }

        public bool Loading { get; set; }
        public string SearchText { get; set; }

        public bool SearchByLocation { get; set; }
        public bool SearchByGps { get; set; }

        public bool DieselSelected { get; set; }
        public bool SuperSelected { get; set; }

        public ObservableCollection<GeocodeResult> Results { get; set; }
        public ObservableCollection<Favorite> Favorites { get; set; }
        public ICommand RemoveFavoriteCommand { get; set; }

        // About
        public string VersionText { get; set; }
        public Uri GitHubUrl { get; set; }
        public Uri PrivacyPolicyUrl { get; set; }
    }
}
