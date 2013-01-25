using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Messaging;
using SprudelSuche.ThirdParty;
using Sprudelsuche.Common;
using Sprudelsuche.Model;
using Sprudelsuche.Portable.Model;
using Sprudelsuche.Services;
using Sprudelsuche.ViewModels;
using Windows.Devices.Geolocation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Core;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Bing.Maps;
using Windows.ApplicationModel.DataTransfer;
using System.Text;
using Windows.Storage.Streams;

namespace Sprudelsuche
{
    public sealed partial class SprudelDetailPage : Sprudelsuche.Common.LayoutAwarePage, IViewModelAwarePage<SprudelDetailViewModel>
    {
        public SprudelDetailViewModel ViewModel { get; set; }

        public SprudelDetailPage()
        {
            this.InitializeComponent();

            if (!ViewModelBase.IsInDesignModeStatic)
            {
                ViewModel = new SprudelDetailViewModel();
                DataContext = ViewModel;
            }
        }

        /// <summary>
        /// Populates the page with content passed during navigation.  Any saved state is also
        /// provided when recreating a page from a prior session.
        /// </summary>
        /// <param name="navigationParameter">The parameter value passed to
        /// <see cref="Frame.Navigate(Type, Object)"/> when this page was initially requested.
        /// </param>
        /// <param name="pageState">A dictionary of state preserved by this page during an earlier
        /// session.  This will be null the first time a page is visited.</param>
        protected override void LoadState(Object navigationParameter, Dictionary<String, Object> pageState)
        {
            DataTransferManager.GetForCurrentView().DataRequested += OnDataRequested;
            myMap.TargetViewChanged += MyMapOnTargetViewChanged;
            Messenger.Default.Register<NotificationMessage>(this, SprudelDetailViewModel.MessengerUpdateMapNotification, (msg) => UpdateMap());

            string navParam = (string)navigationParameter;

            if (navParam.StartsWith(Constants.QuickSearchPrefix))
            {
                ViewModel.ViewMode = SprudelDetailPageViewModeEnum.DisplayCurrentLocation;
                ViewModel.UpdateInProgress = true;
                ViewModel.CurrentLocationFound = false;

                var fueltype = (FuelTypeEnum)Enum.Parse(typeof(FuelTypeEnum), navParam.Substring(Constants.QuickSearchPrefix.Length));

                ViewModel.QueryResult = new GasQueryResult()
                                            {
                                                Name = "Standort wird ermittelt...",
                                                FuelType = fueltype
                                            };

                Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
                                    () => LookupPositionAsync());
            }
            else if (navParam.StartsWith(Constants.ManualSearchPrefix))
            {
                string toDeserialize = navParam.Substring(Constants.ManualSearchPrefix.Length);
                var result = SerializationHelper.DeserializeFromString<GasQueryResult>(toDeserialize);

                ViewModel.ViewMode = SprudelDetailPageViewModeEnum.DisplayTempResult;
                ViewModel.SetViewModelToResult(result);
            }
            else
            {
                Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
                        () => ViewModel.LoadDataAsync(navParam));
            }
        }

        private async Task LookupPositionAsync()
        {
            try
            {
                var currentGeoLocator = new Geolocator();
                var location = await currentGeoLocator.GetGeopositionAsync();

                _positionChangeDetectionActive = true;
                var geocodedLocation = new Location(location.Coordinate.Latitude,
                                                    location.Coordinate.Longitude);
                myMap.SetView(geocodedLocation, 14.0f);

                ViewModel.GeocodeAsync(location.Coordinate.Latitude, location.Coordinate.Longitude);
                return;
            }
            catch (Exception)
            {
                // Potential cause: GetPositionAsync timeout
                ViewModel.UpdateInProgress = false;
            }

            ErrorService.ShowLightDismissError("Ihr Ort konnte nicht erfolgreich erkannt werden");
        }

        // do not start to search for prices until the location has been pinpointed
        private bool _positionChangeDetectionActive = false;

        private async void MyMapOnTargetViewChanged(object sender, TargetViewChangedEventArgs targetViewChangedEventArgs)
        {
            if (_positionChangeDetectionActive)
            {
                _positionChangeDetectionActive = false;

                var bounds = myMap.TargetBounds;
                await ViewModel.DownloadInfoAsync(bounds.South, bounds.West, bounds.North, bounds.East);
            }
        }

        public void UpdateMap()
        {
            var item = ViewModel.QueryResult; // perform work on the current VM item

            myMap.Children.Clear();

            // Only remove existing Pushpins, but otherwise do not modify any aspect of the displayed map
            if (null == item)
            {
                return;
            }

            var geocodedLocation = new Location(item.GeocodeLatitude, item.GeocodeLongitude);
            myMap.SetView(geocodedLocation, 14.0f);

            foreach (var tanke in item.GasStationResults)
            {
                var pin = new Pushpin()
                              {
                                  Text = tanke.UniqueId
                              };
                MapLayer.SetPosition(pin, new Location(tanke.Latitude, tanke.Longitude));
                myMap.Children.Add(pin);
            }
        }

        /// <summary>
        /// Preserves state associated with this page in case the application is suspended or the
        /// page is discarded from the navigation cache.  Values must conform to the serialization
        /// requirements of <see cref="SuspensionManager.SessionState"/>.
        /// </summary>
        /// <param name="pageState">An empty dictionary to be populated with serializable state.</param>
        protected override void SaveState(Dictionary<String, Object> pageState)
        {
            DataTransferManager.GetForCurrentView().DataRequested -= OnDataRequested;
            myMap.TargetViewChanged -= MyMapOnTargetViewChanged;
            myMap.Children.Clear();

            Messenger.Default.Unregister<NotificationMessage>(this, SprudelDetailViewModel.MessengerUpdateMapNotification);
        }

        void OnDataRequested(DataTransferManager sender, DataRequestedEventArgs args)
        {
            var request = args.Request;
            var item = ViewModel.QueryResult;

            if (null == item) return;

            request.Data.Properties.Title = item.DetailTitle;
            request.Data.Properties.Description = String.Format("Spritpreise aktualisiert: {0}", item.LastUpdatedFormatted);

            var stb = new StringBuilder();

            stb.AppendLine();
            stb.Append("TANKSTELLEN\r\n");

            foreach (var tanke in item.GasStationResults)
            {
                stb.AppendFormat("{0} {1}, {2}\r\n{3}\r\n\r\n", tanke.UniqueId, tanke.Name, tanke.Address, tanke.PriceFormatted);
            }

            request.Data.SetText(stb.ToString());
        }

        private async void AppBar_Add_Click(object sender, RoutedEventArgs e)
        {
            bool ok = await ViewModel.AddResultToQueryList();

            if (ok)
                this.Frame.Navigate(typeof(MainPage));
        }
    }
}
