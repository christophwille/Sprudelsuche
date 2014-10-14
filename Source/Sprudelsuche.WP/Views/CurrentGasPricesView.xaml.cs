using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Devices.Geolocation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Maps;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Caliburn.Micro;
using Sprudelsuche.WP.ViewModels;

namespace Sprudelsuche.WP.Views
{
    public sealed partial class CurrentGasPricesView : Page, IHandle<string>
    {
        public CurrentGasPricesView()
        {
            this.InitializeComponent();

            var ea = IoC.Get<IEventAggregator>();
            ea.Subscribe(this);
        }

        CurrentGasPricesViewModel ViewModel { get { return DataContext as CurrentGasPricesViewModel; } }

        private MapControl _gasstationMapControl;

        private async void GasStationMapControl_Loaded(object sender, RoutedEventArgs e)
        {
            _gasstationMapControl = ((MapControl)sender);

            var vm = ViewModel;

            await _gasstationMapControl.TrySetViewAsync(new Geopoint(new BasicGeoposition()
            {
                Latitude = vm.Latitude,
                Longitude = vm.Longitude
            }), 14.0f);

            if (_resetDeferredToLoadEvent) ResetMapElements();
        }

        // http://msdn.microsoft.com/en-us/library/dn792121.aspx
        // http://stackoverflow.com/questions/23701846/how-to-add-pushpin-to-windows-phone-8-1-mapcontrol/24123386#24123386
        public void ResetMapElements()
        {
            var pins = ViewModel.GasStationPins;

            if (null == _gasstationMapControl)
            {
                if (pins.Any()) _resetDeferredToLoadEvent = true;
                return;
            }

            _gasstationMapControl.MapElements.Clear();
            foreach (var element in pins)
            {
                _gasstationMapControl.MapElements.Add(element);
            }
        }

        private bool _resetDeferredToLoadEvent;

        public void Handle(string message)
        {
            if (message == CurrentGasPricesViewModel.ResetMapPinsEvent)
            {
                ResetMapElements();
            }
        }
    }
}
