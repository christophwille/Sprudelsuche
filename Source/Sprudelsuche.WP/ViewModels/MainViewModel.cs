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
using Sprudelsuche.WP.Services;

namespace Sprudelsuche.WP.ViewModels
{
    public partial class MainViewModel : Screen
    {
        private const int MinimumSearchStringLength = 3;

        public Func<IGeocodeProxy> CreateGeocodeProxy { get; set; }
        public bool Loading { get; set; }

        private INavigationService _navigationService;
        private readonly IMessageService _messageService;

        public MainViewModel(INavigationService navigationService, IMessageService messageService)
        {
            _navigationService = navigationService;
            _messageService = messageService;

            CreateGeocodeProxy = () => new NominatimProxy();

            InitializeAbout();
        }

        public string SearchText { get; set; }

        private FuelTypeEnum _selectedFuelType = FuelTypeEnum.Diesel;

        public bool DieselSelected
        {
            get
            {
                return _selectedFuelType == FuelTypeEnum.Diesel;
            }
            set
            {
                _selectedFuelType = FuelTypeEnum.Diesel;
                NotifyOfPropertyChange(() => DieselSelected);
                NotifyOfPropertyChange(() => SuperSelected);
            }
        }

        public bool SuperSelected
        {
            get
            {
                return _selectedFuelType == FuelTypeEnum.Super; 
            }
            set
            {
                _selectedFuelType = FuelTypeEnum.Super;
                NotifyOfPropertyChange(() => DieselSelected);
                NotifyOfPropertyChange(() => SuperSelected);
            }
        }


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

        public bool CanSearch { get { return CanStartSearch(SearchText); } }

        private async Task SearchForMatchesAsync(string searchString)
        {
            if (!CanStartSearch(searchString))
                return;

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
                    await _messageService.ShowAsync("Der Ort konnte nicht gefunden werden");
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

        public void CitySelected(ItemClickEventArgs eventArgs)
        {
            var selected = (GeocodeResult)eventArgs.ClickedItem;

            _navigationService.UriFor<CurrentGasPricesViewModel>()
                .WithParam(vm => vm.FuelType, this._selectedFuelType)
                .WithParam(vm => vm.StationName, selected.Name)
                .WithParam(vm => vm.Longitude, selected.Longitude)
                .WithParam(vm => vm.Latitude, selected.Latitude)
                .Navigate();
        }
    }
}
