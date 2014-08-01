using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Popups;
using Caliburn.Micro;
using Sprudelsuche.Portable;
using Sprudelsuche.Portable.Model;
using Sprudelsuche.Portable.Proxies;
using Sprudelsuche.WP.Models;
using Sprudelsuche.WP.Services;

namespace Sprudelsuche.WP.ViewModels
{
    public class CurrentGasPricesViewModel : Screen
    {
        private readonly IMessageService _messageService;
        private readonly IFavoritesRepository _favoritesRepository;

        // Navigation parameters
        public FuelTypeEnum FuelType { get; set; }
        public string LocationName { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }

        public GasQueryResult QueryResult { get; set; }

        private const double LatitudeBoundingBox = 0.01511;
        private const double LongitudeBoundingBox = 0.01716;

        public Func<IGasPriceInfoProxy> CreateGasPriceInfoProxy { get; set; }
        public bool Loading { get; set; }

        public CurrentGasPricesViewModel(IMessageService messageService, IFavoritesRepository favoritesRepository)
        {
            _messageService = messageService;
            _favoritesRepository = favoritesRepository;

            CreateGasPriceInfoProxy = () => new SpritpreisrechnerProxy();
        }

        protected async override void OnActivate()
        {
            base.OnActivate();

#if !DEBUG
            QueryPricesAsync();
#endif
        }

        public async void RefreshPrices()
        {
            await QueryPricesAsync();
        }

        public async void AddAsFavorite()
        {
            await _favoritesRepository.AddAsync(new Favorite()
            {
                LocationName = this.LocationName,
                FuelType = this.FuelType,
                Latitude = this.Latitude,
                Longitude = this.Longitude
            });
        }

        public async Task QueryPricesAsync()
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
                Loading = true;

                var gasinfoProxy = CreateGasPriceInfoProxy();
                var result = await gasinfoProxy.DownloadAsync(q);

                if (result.Succeeded)
                {
                    QueryResult = result.Result;
                }
                else
                {
                    await _messageService.ShowAsync("Die Preise für den selektierten Ort konnten nicht abgerufen werden");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());

                // No await in catch *yet*
                // InfoMessage = "Die Preise für den selektierten Ort konnten nicht abgerufen werden";
            }
            finally
            {
                Loading = false;
            }
        }
    }
}
