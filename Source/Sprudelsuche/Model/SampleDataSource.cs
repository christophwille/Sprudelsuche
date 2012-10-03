using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sprudelsuche.Portable.Model;

namespace Sprudelsuche.Model
{
    public sealed class SampleDataSource
    {
        private static SampleDataSource _sampleDataSource = new SampleDataSource();

        private readonly ObservableCollection<GasQueryResult> _suchErgebnisse = new ObservableCollection<GasQueryResult>();
        public ObservableCollection<GasQueryResult> SuchErgebnisse
        {
            get { return _suchErgebnisse; }
        }

        private readonly ObservableCollection<GeocodeResult> _geocodeResults = new ObservableCollection<GeocodeResult>();
        public ObservableCollection<GeocodeResult> GeocodeResults
        {
            get { return _geocodeResults; }
        }

        private readonly ObservableCollection<FuelTypeAction> _fuelTypeActions = new ObservableCollection<FuelTypeAction>();
        public ObservableCollection<FuelTypeAction> FuelTypeActions
        {
            get { return _fuelTypeActions; }
        }

        private readonly List<MainPageAction> _mainpageActions = new List<MainPageAction>();
        public List<MainPageAction> MainPageActions
        {
            get { return _mainpageActions; }
        }

        public SampleDataSource()
        {
            var se = new GasQueryResult()
                         {
                             UniqueId = "S1-Leoben-TestData",
                             FuelType = FuelTypeEnum.Diesel,
                             Longitude1 = 15.051175814518464,
                             Latitude1 = 47.36476416660497,
                             Longitude2 = 15.13108418548611,
                             Latitude2 = 47.38801327070198,
                             GeocodeLongitude = 15.093971,
                             GeocodeLatitude = 47.3827604,
                             Name = "Leoben / Steiermark",
                             LastUpdated = default(DateTime)
                         };

            se.GasStationResults.Add(new GasStationResult()
                                               {
                                                   Name = "Beispieldaten: Markentankstelle",
                                                   PostalCode = "8700",
                                                   Price = 0.0f
                                               });

            SuchErgebnisse.Add(se);

            se = new GasQueryResult()
            {
                UniqueId = "S2-Leoben-TestData",
                FuelType = FuelTypeEnum.Super,
                Longitude1 = 15.051175814518464,
                Latitude1 = 47.36476416660497,
                Longitude2 = 15.13108418548611,
                Latitude2 = 47.38801327070198,
                GeocodeLongitude = 15.093971,
                GeocodeLatitude = 47.3827604,
                Name = "Leoben / Steiermark",
                LastUpdated = default(DateTime)
            };

            se.GasStationResults.Add(new GasStationResult()
            {
                Name = "Beispieldaten: Freie Tankstelle",
                PostalCode = "8700",
                Price = 0.0f
            });


            SuchErgebnisse.Add(se);


            _geocodeResults.Add(new GeocodeResult()
                                      {
                                          Name = "Leoben / Austria",
                                          Latitude = 47.3827604,
                                          Longitude = 15.093971
                                      });

            // Fuel Actions
            _fuelTypeActions.Add(new FuelTypeAction()
                                     {
                                         FuelType = FuelTypeEnum.Diesel
                                     });
            _fuelTypeActions.Add(new FuelTypeAction()
                                     {
                                         FuelType = FuelTypeEnum.Super
                                     });

            // Main Page Actions 
            _mainpageActions = MainPageAction.GenerateMainPageActions();
        }
    }
}
