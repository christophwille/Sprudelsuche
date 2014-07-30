using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Caliburn.Micro;
using Sprudelsuche.Portable;
using Sprudelsuche.Portable.Model;
using Sprudelsuche.Portable.Proxies;

namespace Sprudelsuche.WP.ViewModels
{
    public class CurrentGasPricesViewModel : Screen
    {
        private readonly INavigationService _navigationService;
        public FuelTypeEnum FuelType { get; set; }
        public string StationName { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public GasQueryResult QueryResult { get; set; }

        private const double LatitudeBoundingBox = 0.01511;
        private const double LongitudeBoundingBox = 0.01716;

        public Func<IGasPriceInfoProxy> CreateGasPriceInfoProxy { get; set; }

        public CurrentGasPricesViewModel(INavigationService navigationService)
        {
            _navigationService = navigationService;
            CreateGasPriceInfoProxy = () => new SpritpreisrechnerProxy();
        }

        public async Task QueryForPrices()
        {
            QueryResult = null;

            var q = new GasQuery()
            {
                FuelType = this.FuelType,
                GeocodeLatitude = this.Latitude,
                GeocodeLongitude = this.Longitude,
                UniqueId = Guid.NewGuid().ToString(),
                Latitude1 = this.Latitude - LatitudeBoundingBox,
                Latitude2 = this.Latitude + LatitudeBoundingBox,
                Longitude1 = this.Longitude - LongitudeBoundingBox,
                Longitude2 = this.Longitude + LongitudeBoundingBox
            };

            try
            {
                var gasinfoProxy = CreateGasPriceInfoProxy();
                var result = await gasinfoProxy.DownloadAsync(q);

                if (result.Succeeded)
                {
                    QueryResult = result.Result;
                }
                else
                {
                    // InfoMessage = "Die Preise für den selektierten Ort konnten nicht abgerufen werden";
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
                // InfoMessage = "Die Preise für den selektierten Ort konnten nicht abgerufen werden";
            }
        }
    }
}
