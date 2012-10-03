using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using SprudelSuche.ThirdParty;
using Sprudelsuche.Model;
using Sprudelsuche.Portable;
using Sprudelsuche.Portable.Model;
using Sprudelsuche.Services;
using Sprudelsuche.WinRT.Proxies;
using Windows.Devices.Geolocation;
using Windows.Foundation;
using Windows.UI.Popups;

namespace Sprudelsuche.ViewModels
{
    public class AddSprudelSucheViewModel : SprudelViewModelBase
    {
        public const string MessengerSetViewNotification = "SetView";

        public Func<IGeocodeProxy> CreateGeocodeProxy { get; set; }
        public Func<IGasPriceInfoProxy> CreateGasPriceInfoProxy { get; set; }

        public AddSprudelSucheViewModel()
            : base()
        {
            FuelTypeActions = new List<FuelTypeAction>()
                                  {
                                      new FuelTypeAction()
                                          {
                                              FuelType = FuelTypeEnum.Diesel
                                          },
                                      new FuelTypeAction()
                                          {
                                              FuelType = FuelTypeEnum.Super
                                          }
                                  };

            CreateGeocodeProxy = () => new NominatimProxy();
            CreateGasPriceInfoProxy = () => new SpritpreisrechnerProxy();
        }

        public const string GeocodeResultsPropertyName = "GeocodeResults";
        private ObservableCollection<GeocodeResult> _geocodeResults = new ObservableCollection<GeocodeResult>();
        public ObservableCollection<GeocodeResult> GeocodeResults
        {
            get { return _geocodeResults; }
            set
            {
                Set(GeocodeResultsPropertyName, ref _geocodeResults, value);
            }
        }

        private RelayCommand _searchCommand;
        public RelayCommand SearchCommand
        {
            get
            {
                return _searchCommand
                    ?? (_searchCommand = new RelayCommand(
                        async () => await GeocodeAsync(),
                        () => CanSearch));
            }
        }

        public bool CanSearch
        {
            get { return SearchText.Trim().Length >= 3; }
        }

        public const string SearchTextPropertyName = "SearchText";
        private string _searchText = "";

        public string SearchText
        {
            get
            {
                return _searchText;
            }
            set
            {
                Set(SearchTextPropertyName, ref _searchText, value);
                SearchCommand.RaiseCanExecuteChanged();
            }
        }

        public const string InfoMessagePropertyName = "InfoMessage";
        private string _infoMessageText = "";

        public string InfoMessage
        {
            get
            {
                return _infoMessageText;
            }
            set
            {
                Set(InfoMessagePropertyName, ref _infoMessageText, value);
            }
        }

        // UpdateSourceTrigger HACK
        public Action<string> UpdateBoundSearchTextProperty
        {
            get { return new Action<string>((value) => SearchText = value); }
        }

        private async Task GeocodeAsync()
        {
            ResetModelPriorToSearch();
            InfoMessage = "";

            try
            {
                var geocodeProxy = CreateGeocodeProxy();
                var result = await geocodeProxy.ExecuteQuery(SearchText);

                if (result.Count > 0)
                {
                    foreach (var res in result)
                        GeocodeResults.Add(res);

                    SelectedGeocodeResult = GeocodeResults.FirstOrDefault();
                }
                else
                {
                    InfoMessage = "Der Ort konnte nicht gefunden werden";
                }
            }
            catch (Exception)
            {
                InfoMessage = "Die Suchanfrage ist festgeschlagen";
            }
        }

        private void ResetModelPriorToSearch()
        {
            SelectedGeocodeResult = null;
            GeocodeResults.Clear();

            SetLongLatRectangle(0.0f, 0.0f, 0.0f, 0.0f);
        }

        public void SetLongLatRectangle(double lat1, double long1, double lat2, double long2)
        {
            _queryParameters.Latitude1 = lat1;
            _queryParameters.Longitude1 = long1;
            _queryParameters.Latitude2 = lat2;
            _queryParameters.Longitude2 = long2;

            RaisePropertyChanged(AddAllowedPropertyName);
        }

        public const string SelectedGeocodeResultPropertyName = "SelectedGeocodeResult";
        private GeocodeResult _selectedGeocodeResult = null;
        public GeocodeResult SelectedGeocodeResult
        {
            get { return _selectedGeocodeResult; }
            set
            {
                Set(SelectedGeocodeResultPropertyName, ref _selectedGeocodeResult, value);

                if (null != value)
                {
                    Messenger.Default.Send<NotificationMessage>(new NotificationMessage(this, "ignore"), MessengerSetViewNotification);
                }

                RaisePropertyChanged(AddAllowedPropertyName);
            }
        }

        private readonly GasQuery _queryParameters = new GasQuery();

        public const string FuelTypeActionsPropertyName = "FuelTypeActions";
        private List<FuelTypeAction> _fuelTypesActions = null;
        public List<FuelTypeAction> FuelTypeActions
        {
            get { return _fuelTypesActions; }
            protected set { Set(FuelTypeActionsPropertyName, ref _fuelTypesActions, value); }
        }

        public const string AddAllowedPropertyName = "AddAllowed";
        public bool AddAllowed
        {
            get
            {
                return SelectedGeocodeResult != null && _queryParameters.Latitude1 != 0.0f;
            }
        }

        public async Task QueryGeocodedLocationAsync(FuelTypeEnum fuelType)
        {
            _queryParameters.FuelType = fuelType;
            _queryParameters.Name = SelectedGeocodeResult.Name;
            _queryParameters.GeocodeLatitude = SelectedGeocodeResult.Latitude;
            _queryParameters.GeocodeLongitude = SelectedGeocodeResult.Longitude;

            _queryParameters.UniqueId = Guid.NewGuid().ToString();

            AddOperationInProgress = true;
            InfoMessage = "";

            try
            {
                var gasinfoProxy = CreateGasPriceInfoProxy();
                var result = await gasinfoProxy.Download(_queryParameters);

                if (result.Succeeded)
                {
                    string serializedResult = SerializationHelper.SerializeToString(result.Result);
                    NavigationService.Navigate<SprudelDetailPage>(Constants.ManualSearchPrefix + serializedResult);
                }
                else
                {
                    InfoMessage = "Die Preise für den selektierten Ort konnten nicht abgerufen werden";
                }
            }
            catch (Exception)
            {
                InfoMessage = "Die Preise für den selektierten Ort konnten nicht abgerufen werden";
            }

            AddOperationInProgress = false;
        }

        public const string AddOperationInProgressPropertyName = "AddOperationInProgress";
        private bool _addOperationInProgress = false;

        public bool AddOperationInProgress
        {
            get
            {
                return _addOperationInProgress;
            }
            set
            {
                Set(AddOperationInProgressPropertyName, ref _addOperationInProgress, value);
            }
        }

        public void LoadState(AddSprudelSuchePageState state)
        {
            SearchText = state.SearchText;
            GeocodeResults = new ObservableCollection<GeocodeResult>(state.GeocodeResults);

            if (!String.IsNullOrWhiteSpace(state.SelectedGeocodeResultUniqueId))
            {
                SelectedGeocodeResult = GeocodeResults.FirstOrDefault(r => r.UniqueId == state.SelectedGeocodeResultUniqueId);
            }
        }

        public AddSprudelSuchePageState SaveState()
        {
            var state = new AddSprudelSuchePageState()
                            {
                                SearchText = this.SearchText,
                                GeocodeResults = this.GeocodeResults.ToList(),
                                SelectedGeocodeResultUniqueId = this.SelectedGeocodeResult != null ? this.SelectedGeocodeResult.UniqueId : ""
                            };

            return state;
        }
    }
}
