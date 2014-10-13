using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls.Maps;
using Sprudelsuche.Portable.Model;

namespace Sprudelsuche.WP.ViewModels
{
    // Make sure the design-time vm matches the runtime vm for full Blend fidelity in creating the bindings
    public interface ICurrentGasPricesViewModelBindings
    {
        FuelTypeEnum FuelType { get; set; }
        string LocationName { get; set; }
        GasQueryResult QueryResult { get; set; }
        bool Loading { get; set; }


        string MapAuthenticationToken { get; }
        List<MapIcon> GasStationPins { get; set; } 
    }
}
