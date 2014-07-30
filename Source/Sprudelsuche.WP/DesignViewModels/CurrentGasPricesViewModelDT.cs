using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sprudelsuche.Portable.Model;

namespace Sprudelsuche.WP.DesignViewModels
{
    class CurrentGasPricesViewModelDT
    {
        public CurrentGasPricesViewModelDT()
        {
            FuelType = FuelTypeEnum.Diesel;
            StationName = "Leoben";

            QueryResult = new GasQueryResult();
            QueryResult.GasStationResults.Add(new GasStationResult()
            {
                Name = "Fantasy Brand Gas Station",
                Price = 1.28
            });
        }

        public FuelTypeEnum FuelType { get; set; }
        public string StationName { get; set; }
        public GasQueryResult QueryResult { get; set; }
    }
}
