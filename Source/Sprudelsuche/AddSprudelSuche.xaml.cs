using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using Bing.Maps;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Messaging;
using SprudelSuche.ThirdParty;
using Sprudelsuche.Common;
using Sprudelsuche.Model;
using Sprudelsuche.Portable.Model;
using Sprudelsuche.WinRT.Proxies;
using Sprudelsuche.ViewModels;
using Windows.Devices.Geolocation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.System;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

namespace Sprudelsuche
{
    public sealed partial class AddSprudelSuche : Sprudelsuche.Common.LayoutAwarePage, IViewModelAwarePage<AddSprudelSucheViewModel>
    {
        public AddSprudelSucheViewModel ViewModel { get; set; }

        public AddSprudelSuche()
        {
            this.InitializeComponent();

            if (!ViewModelBase.IsInDesignModeStatic)
            {
                ViewModel = new AddSprudelSucheViewModel();
                DataContext = ViewModel;
            }
        }

        protected override void LoadState(Object navigationParameter, Dictionary<String, Object> pageState)
        {
            myMap.TargetViewChanged += MyMapOnTargetViewChanged;
            Messenger.Default.Register<NotificationMessage>(this, AddSprudelSucheViewModel.MessengerSetViewNotification, (msg) => SetView());

            if (pageState != null && pageState.ContainsKey(Constants.AddSprudelSuchePageState))
            {
                string serializedState = pageState[Constants.AddSprudelSuchePageState].ToString();
                var state = SerializationHelper.DeserializeFromString<AddSprudelSuchePageState>(serializedState);

                ViewModel.LoadState(state);
            }
        }

        protected override void SaveState(Dictionary<String, Object> pageState)
        {
            myMap.TargetViewChanged -= MyMapOnTargetViewChanged;
            Messenger.Default.Unregister<NotificationMessage>(this, AddSprudelSucheViewModel.MessengerSetViewNotification);

            string serializedState = SerializationHelper.SerializeToString(ViewModel.SaveState());
            pageState[Constants.AddSprudelSuchePageState] = serializedState;
        }

        private void MyMapOnTargetViewChanged(object sender, TargetViewChangedEventArgs targetViewChangedEventArgs)
        {
            var bounds = myMap.TargetBounds;
            ViewModel.SetLongLatRectangle(bounds.South, bounds.West, bounds.North, bounds.East);
        }

        public void SetView()
        {
            var item = ViewModel.SelectedGeocodeResult;

            var location = new Location(item.Latitude, item.Longitude);
            myMap.SetView(location, 14.0f);
        }

        private async void QueryLocationAndGoToDetailsPage(object sender, ItemClickEventArgs e)
        {
            var selectedAction = (FuelTypeAction)e.ClickedItem;

            Dispatcher.RunAsync(CoreDispatcherPriority.Normal, 
                () => ViewModel.QueryGeocodedLocationAsync(selectedAction.FuelType));
        }

        private async void LocationTextBox_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == VirtualKey.Enter)
            {
                if (ViewModel.CanSearch)
                {
                    ViewModel.SearchCommand.Execute(null);
                }
            }
        }
    }
}
