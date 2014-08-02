using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sprudelsuche.Portable.Model;
using Sprudelsuche.WP.ViewModels;

namespace Sprudelsuche.WP.DesignViewModels
{
    public class CurrentGasPricesViewModelDT : ICurrentGasPricesViewModelBindings
    {
        public CurrentGasPricesViewModelDT()
        {
            FuelType = FuelTypeEnum.Diesel;
            LocationName = "Leoben";

            QueryResult = new GasQueryResult();
            QueryResult.GasStationResults.Add(new GasStationResult()
            {
                Name = "Fantasy Brand Gas Station",
                Price = 1.28,
                City = "Leoben",
                PostalCode = "8700",
                Street = "Some street 21"
            });
        }

        public FuelTypeEnum FuelType { get; set; }
        public string LocationName { get; set; }
        public GasQueryResult QueryResult { get; set; }
        public bool Loading { get; set; }
    }
}
