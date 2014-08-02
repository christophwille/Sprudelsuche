using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Caliburn.Micro;
using Newtonsoft.Json;
using Sprudelsuche.Portable;
using Sprudelsuche.Portable.Model;
using Sprudelsuche.Portable.Proxies;
using Sprudelsuche.WP.Common;
using Sprudelsuche.WP.Models;
using Sprudelsuche.WP.Services;

namespace Sprudelsuche.WP.ViewModels
{
    public partial class MainViewModel : Screen, IStateEnabledViewModel
    {
        private const int MinimumSearchStringLength = 3;

        public Func<IGeocodeProxy> CreateGeocodeProxy { get; set; }
        public bool Loading { get; set; }

        private readonly INavigationService _navigationService;
        private readonly IMessageService _messageService;
        private readonly IFavoritesRepository _favoritesRepository;

        public MainViewModel(INavigationService navigationService, IMessageService messageService, IFavoritesRepository favoritesRepository)
        {
            _navigationService = navigationService;
            _messageService = messageService;
            _favoritesRepository = favoritesRepository;

            CreateGeocodeProxy = () => new NominatimProxy();

            InitializeAbout();

            RemoveFavoriteCommand = new RelayCommand<Favorite>(async (f) => await RemoveFavorite(f));
        }

        protected async override void OnActivate()
        {
            base.OnActivate();

            var favorites = await _favoritesRepository.LoadAsync();

            if (null != favorites)
            {
                Favorites = new ObservableCollection<Favorite>(favorites);
            }
        }

        public string SearchText { get; set; }

        private FuelTypeEnum _selectedFuelType = FuelTypeEnum.Diesel;

        public bool DieselSelected
        {
            get { return _selectedFuelType == FuelTypeEnum.Diesel; }
            set { SelectFuelType(FuelTypeEnum.Diesel); }
        }

        public bool SuperSelected
        {
            get { return _selectedFuelType == FuelTypeEnum.Super; }
            set { SelectFuelType(FuelTypeEnum.Super); }
        }

        private void SelectFuelType(FuelTypeEnum ft)
        {
            _selectedFuelType = ft;
            NotifyOfPropertyChange(() => DieselSelected);
            NotifyOfPropertyChange(() => SuperSelected);
        }

        private bool IsSearchTextValid(string searchString)
        {
            if (String.IsNullOrWhiteSpace(searchString)) return false;

            return (searchString.Length >= MinimumSearchStringLength);
        }

        public ObservableCollection<GeocodeResult> Results { get; set; }

        public async void Search()
        {
            await SearchForMatchesAsync(SearchText);
        }

        public bool CanSearch { get { return IsSearchTextValid(SearchText); } }

        private async Task SearchForMatchesAsync(string searchString)
        {
            if (!IsSearchTextValid(searchString))
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
                .WithParam(vm => vm.LocationName, selected.Name)
                .WithParam(vm => vm.Longitude, selected.Longitude)
                .WithParam(vm => vm.Latitude, selected.Latitude)
                .Navigate();
        }

        public ObservableCollection<Favorite> Favorites { get; set; }

        public void FavoriteSelected(ItemClickEventArgs eventArgs)
        {
            var selected = (Favorite)eventArgs.ClickedItem;

            _navigationService.UriFor<CurrentGasPricesViewModel>()
                .WithParam(vm => vm.FuelType, selected.FuelType)
                .WithParam(vm => vm.LocationName, selected.LocationName)
                .WithParam(vm => vm.Longitude, selected.Longitude)
                .WithParam(vm => vm.Latitude, selected.Latitude)
                .Navigate();
        }

        public ICommand RemoveFavoriteCommand { get; private set; }
        public async Task RemoveFavorite(Favorite toRemove)
        {
            var favorites = await _favoritesRepository.RemoveAsync(toRemove);
            Favorites = new ObservableCollection<Favorite>(favorites);
        }

        #region Page State Management

        private const string PageStateName = "MainViewModelState";
        public void LoadState(Dictionary<string, object> pageState)
        {
            if (pageState != null && pageState.ContainsKey(PageStateName))
            {
                string json = pageState[PageStateName].ToString();
                var state = JsonConvert.DeserializeObject<MainViewModelState>(json);
                if (null == state) return;

                this.SearchText = state.SearchText;
                SelectFuelType(state.FuelType);
                if (null != state.Results) this.Results = new ObservableCollection<GeocodeResult>(state.Results);
            }
        }

        public void SaveState(Dictionary<string, object> pageState)
        {
            var state = new MainViewModelState()
            {
                SearchText = this.SearchText,
                FuelType = this._selectedFuelType,
            };
            if (null != Results) state.Results = Results.ToList();

            string json = JsonConvert.SerializeObject(state);

            pageState[PageStateName] = json;
        }

        #endregion
    }
}
