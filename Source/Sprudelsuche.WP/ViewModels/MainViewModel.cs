using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Caliburn.Micro;
using Sprudelsuche.Portable;
using Sprudelsuche.Portable.Model;
using Sprudelsuche.Portable.Proxies;

namespace Sprudelsuche.WP.ViewModels
{
    public partial class MainViewModel : Screen
    {
        private const int MinimumSearchStringLength = 3;

        public Func<IGeocodeProxy> CreateGeocodeProxy { get; set; }
        public bool Loading { get; set; }

        private INavigationService _navigationService;

        public MainViewModel(INavigationService navigationService)
        {
            _navigationService = navigationService;

            CreateGeocodeProxy = () => new NominatimProxy();

            InitializeAbout();
        }

        public string SearchText { get; set; }


        private bool CanStartSearch(string searchString)
        {
            if (String.IsNullOrWhiteSpace(searchString)) return false;

            return (searchString.Length >= MinimumSearchStringLength);
        }

        public ObservableCollection<GeocodeResult> Results { get; set; }

        public async void Search()
        {
            await SearchForMatchesAsync(SearchText);
        }

        public bool CanSearch { get { return CanStartSearch(SearchText); }}

        private async Task SearchForMatchesAsync(string searchString)
        {
            if (!CanStartSearch(searchString))
                return;

            SelectedResult = null;
            Results = null;

            try
            {
                Loading = true;
                var geocodeProxy = CreateGeocodeProxy();
                var result = await geocodeProxy.ExecuteQuery(searchString);

                if (result.Count > 0)
                {
                    Results = new ObservableCollection<GeocodeResult>(result);
                }
                else
                {
                    Debug.WriteLine("Der Ort konnte nicht gefunden werden");
                    // InfoMessage = "Der Ort konnte nicht gefunden werden";
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
                // InfoMessage = "Die Suchanfrage ist festgeschlagen";
            }
            finally
            {
                Loading = false;
            }
        }

        public GeocodeResult SelectedResult { get; set; }

        public void DieselAnzeigen()
        {
            NavigateTo(SelectedResult, FuelTypeEnum.Diesel);
        }

        public bool CanDieselAnzeigen { get { return null != SelectedResult; } }

        public void SuperAnzeigen()
        {
            NavigateTo(SelectedResult, FuelTypeEnum.Super);
        }

        public bool CanSuperAnzeigen { get { return null != SelectedResult; } }

        private void NavigateTo(GeocodeResult selected, FuelTypeEnum fuelType)
        {
            _navigationService.UriFor<CurrentGasPricesViewModel>()
                .WithParam(vm => vm.FuelType, fuelType)
                .WithParam(vm => vm.StationName, selected.Name)
                .WithParam(vm => vm.Longitude, selected.Longitude)
                .WithParam(vm => vm.Latitude, selected.Latitude)
                .Navigate();
        }
    }
}
