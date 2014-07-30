using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using Sprudelsuche.Common8;
using Sprudelsuche.Model;
using Sprudelsuche.Portable;
using Sprudelsuche.Portable.Model;
using Sprudelsuche.Services;
using Sprudelsuche.Portable.Proxies;
using Windows.ApplicationModel.Resources.Core;
using Windows.UI;
using Windows.UI.Popups;
using Windows.UI.StartScreen;
using Windows.UI.Xaml;

namespace Sprudelsuche.ViewModels
{
    public class SprudelDetailViewModel : SprudelViewModelBase
    {
        public const string MessengerUpdateMapNotification = "UpdateMap";

        public Func<IGeocodeProxy> CreateGeocodeProxy { get; set; }
        public Func<INotifyGasQueryResultChanged> CreateResultChangedNotification { get; set; }
        public Func<IGasPriceInfoProxy> CreateGasPriceInfoProxy { get; set; }

        public SprudelDetailViewModel()
            : base()
        {
            CreateResultChangedNotification = () => new NotifyTileOfGasQueryResultChanged();
            CreateGeocodeProxy = () => new NominatimProxy();
            CreateGasPriceInfoProxy = () => new SpritpreisrechnerProxy();
        }

        public const string ViewModePropertyName = "ViewMode";
        private SprudelDetailPageViewModeEnum _viewMode = SprudelDetailPageViewModeEnum.DisplayFavorite;

        public SprudelDetailPageViewModeEnum ViewMode
        {
            get
            {
                return _viewMode;
            }
            set
            {
                Set(ViewModePropertyName, ref _viewMode, value);

                RaisePropertyChanged(IsInDetailsModePropertyName);
                RaisePropertyChanged(IsInDetailsModePropertyName);
                RaisePropertyChanged(AddToFavoritesEnabledPropertyName);
                RaisePropertyChanged(RefreshEnabledPropertyName);
            }
        }

        public const string IsInTempLocationModePropertyName = "IsInTempLocationMode";
        public bool IsInTempLocationMode
        {
            get { return ViewMode != SprudelDetailPageViewModeEnum.DisplayFavorite; }
        }

        public const string IsInDetailsModePropertyName = "IsInDetailsMode";
        public bool IsInDetailsMode
        {
            get { return ViewMode == SprudelDetailPageViewModeEnum.DisplayFavorite; }
        }

        public const string CurrentLocationFoundPropertyName = "CurrentLocationFound";
        private bool _currentLocationFound = false;

        public bool CurrentLocationFound
        {
            get
            {
                return _currentLocationFound;
            }
            set
            {
                Set(CurrentLocationFoundPropertyName, ref _currentLocationFound, value);

                RaisePropertyChanged(AddToFavoritesEnabledPropertyName);
                RaisePropertyChanged(RefreshEnabledPropertyName);
            }
        }

        public const string AddToFavoritesEnabledPropertyName = "AddToFavoritesEnabled";
        public bool AddToFavoritesEnabled
        {
            get
            {
                if (ViewMode == SprudelDetailPageViewModeEnum.DisplayFavorite)
                    return false;

                if (ViewMode == SprudelDetailPageViewModeEnum.DisplayTempResult)
                    return true;

                // if (ViewMode == SprudelDetailPageViewModeEnum.DisplayCurrentLocation)
                return _currentLocationFound;
            }
        }

        public const string RefreshEnabledPropertyName = "RefreshEnabled";
        public bool RefreshEnabled
        {
            get
            {
                // In both temp and favorite mode, we can refresh without additional checks
                if (ViewMode != SprudelDetailPageViewModeEnum.DisplayCurrentLocation)
                    return true;

                // if (ViewMode == SprudelDetailPageViewModeEnum.DisplayCurrentLocation)
                return _currentLocationFound;
            }
        }

        public const string UpdateInProgressPropertyName = "UpdateInProgress";
        private bool _updateInProgress = false;

        public bool UpdateInProgress
        {
            get
            {
                return _updateInProgress;
            }
            set
            {
                Set(UpdateInProgressPropertyName, ref _updateInProgress, value);
            }
        }

        public async Task LoadDataAsync(string uniqueId)
        {
            var item = await CreateSprudelRepository().LoadResultAsync((String)uniqueId);

            SetViewModelToResult(item);
        }

        public void SetViewModelToResult(GasQueryResult item)
        {
            QueryResult = item;
            SendUpdateMapNotification();
        }

        private void SendUpdateMapNotification()
        {
            Messenger.Default.Send<NotificationMessage>(new NotificationMessage(this, "ignore"), MessengerUpdateMapNotification);
        }

        public async Task GeocodeAsync(double latitude, double longitude)
        {
            // Provide interim information
            _geocodeResult = new GeocodeResult()
                                 {
                                     Latitude = latitude,
                                     Longitude = longitude,
                                     Name = String.Format("Lat: {0}, Lon: {1}", latitude, longitude)
                                 };
            SetQueryResultToGeocodeResult(QueryResult);

            try
            {
                var geocodeProxy = CreateGeocodeProxy();
                var result = await geocodeProxy.ReverseGeocode(latitude, longitude);

                _geocodeResult = result;
                SetQueryResultToGeocodeResult(QueryResult);
            }
            catch (Exception)
            {
                ErrorService.ShowLightDismissError("Ihr Ort konnte nicht erfolgreich erkannt werden");
            }
        }

        private GeocodeResult _geocodeResult = null;

        private void SetQueryResultToGeocodeResult(GasQueryResult currentQueryResult)
        {
            if (currentQueryResult != null)
            {
                var result = new GasQueryResult(currentQueryResult);

                if (_geocodeResult != null)
                {
                    result.GeocodeLatitude = _geocodeResult.Latitude;
                    result.GeocodeLongitude = _geocodeResult.Longitude;
                    result.Name = _geocodeResult.Name;
                }

                QueryResult = result;
                return;
            }

            QueryResult = currentQueryResult;
        }

        public async Task DownloadInfoAsync(double lat1, double long1, double lat2, double long2)
        {
            var fueltype = QueryResult.FuelType;
            var parameter = new GasQuery(Guid.NewGuid().ToString(), fueltype, long1, lat1, long2, lat2);

            try
            {
                var gasinfoProxy = CreateGasPriceInfoProxy();
                var result = await gasinfoProxy.DownloadAsync(parameter);

                if (result.Succeeded)
                {
                    SetQueryResultToGeocodeResult(result.Result);
                    SendUpdateMapNotification();

                    CurrentLocationFound = true;
                }
                else
                {
                    ErrorService.ShowLightDismissError("Die Preisinformationen konnten nicht ermittelt werden");
                }
            }
            catch (Exception)
            {
                ErrorService.ShowLightDismissError("Die Preisinformationen konnten nicht ermittelt werden");
            }

            UpdateInProgress = false;
        }

        public const string QueryResultPropertyName = "QueryResult";
        private GasQueryResult _gasQueryResult = null;
        public GasQueryResult QueryResult
        {
            get
            {
                return _gasQueryResult;
            }
            set
            {
                Set(QueryResultPropertyName, ref _gasQueryResult, value);
                PinCommand.RaiseCanExecuteChanged();
            }
        }

        public const string PinOrUnpinStylePropertyName = "PinOrUnpinStyle";
        public Style PinOrUnpinStyle
        {
            get
            {
                bool isCurrentlyPinned = SecondaryTile.Exists(QueryResult.UniqueId);
                if (!isCurrentlyPinned)
                {
                    return ResourceService.Get<Style>("PinAppBarButtonStyle");
                }
                else
                {
                    return ResourceService.Get<Style>("UnpinAppBarButtonStyle");
                }
            }
        }

        public const string PinOrUnpinTextPropertyName = "PinOrUnpinText";
        public string PinOrUnpinText
        {
            get
            {
                bool isCurrentlyPinned = SecondaryTile.Exists(QueryResult.UniqueId);
                if (!isCurrentlyPinned)
                {
                    return "An \"Start\" anheften";
                }
                else
                {
                    return "Von \"Start\" lösen";
                }
            }
        }

        private RelayCommand _pinCommand;
        public RelayCommand PinCommand
        {
            get
            {
                return _pinCommand
                    ?? (_pinCommand = new RelayCommand(
                        async () => await PinUnpinAsync(),
                        () => QueryResult != null));
            }
        }

        private async Task PinUnpinAsync()
        {
            string tileId = QueryResult.UniqueId;
            bool isCurrentlyPinned = SecondaryTile.Exists(tileId);

            if (isCurrentlyPinned)
            {
                var secondaryTile = new SecondaryTile(tileId);
                bool isUnpinned = await secondaryTile.RequestDeleteAsync();
            }
            else
            {
                Uri logo = new Uri(Constants.SquareTileLogo);
                Uri wideLogo = new Uri(Constants.WideTileLogo);

                string tileActivationArguments = tileId;

                var secondaryTile = new SecondaryTile(tileId,
                                                                QueryResult.Name,
                                                                QueryResult.DetailTitle,
                                                                tileActivationArguments,
                                                                TileOptions.ShowNameOnLogo | TileOptions.ShowNameOnWideLogo,
                                                                logo, 
                                                                wideLogo);

                secondaryTile.ForegroundText = ForegroundText.Light;

                if (QueryResult.FuelType == FuelTypeEnum.Diesel)
                {
                    secondaryTile.BackgroundColor = Colors.Black;
                }
                else
                {
                    secondaryTile.BackgroundColor = Colors.DarkGreen;
                }

                bool isPinned = await secondaryTile.RequestCreateAsync();

                if (isPinned)
                {
                    CreateResultChangedNotification().NotifyGasQueryResultChanged(QueryResult);
                }
            }

            RaisePropertyChanged(PinOrUnpinStylePropertyName);
            RaisePropertyChanged(PinOrUnpinTextPropertyName);
        }

        public async Task<bool> AddResultToQueryList()
        {
            UpdateInProgress = true;
            var succeeded = await CreateSprudelRepository().AddResultAsync(QueryResult);
            UpdateInProgress = false;

            return succeeded;
        }

        private RelayCommand _refreshDataCommand;
        public RelayCommand RefreshDataCommand
        {
            get
            {
                return _refreshDataCommand
                    ?? (_refreshDataCommand = new RelayCommand(
                        async () => await RefreshDataAsync()));
            }
        }

        public async Task RefreshDataAsync()
        {
            UpdateInProgress = true;

            if (ViewMode == SprudelDetailPageViewModeEnum.DisplayFavorite)
            {
                var repo = CreateSprudelRepository();
                var newInfos = await repo.RefreshAsync();

                if (newInfos.Succeeded)
                {
                    QueryResult = newInfos.Results.Single(r => r.UniqueId == QueryResult.UniqueId);
                }
                else
                {
                    ErrorService.ShowLightDismissError("Refresh der Preise ist fehlgeschlagen");
                }
            }
            else
            {
                var gasinfoProxy = CreateGasPriceInfoProxy();
                var result = await gasinfoProxy.DownloadAsync(QueryResult);

                if (result.Succeeded)
                {
                    QueryResult = result.Result;
                }
                else
                {
                    ErrorService.ShowLightDismissError("Refresh der Preise ist fehlgeschlagen");
                }
            }

            UpdateInProgress = false;
        }
    }
}
