using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Xml.Serialization;

namespace Sprudelsuche.Portable.Model
{
    public class GasQueryResult : GasQuery
    {
        public GasQueryResult():base()
        {
        }

        public GasQueryResult(GasQuery parameterOriginal):base(parameterOriginal)
        {
        }

        public GasQueryResult(GasQueryResult parameterOriginal)
            : this(parameterOriginal, parameterOriginal.GasStationResults.ToList(), parameterOriginal.LastUpdated)
        {
        }

        public GasQueryResult(GasQuery parameterOriginal, List<GasStationResult> gasStationResults, DateTime lastUpdated)
            : base(parameterOriginal)
        {
            _gasstationQueryResults = new ObservableCollection<GasStationResult>(gasStationResults);
            _lastUpdated = lastUpdated;
        }

        private ObservableCollection<GasStationResult> _gasstationQueryResults = new ObservableCollection<GasStationResult>();
        public ObservableCollection<GasStationResult> GasStationResults
        {
            get { return _gasstationQueryResults; }
        }

        private DateTime _lastUpdated = default(DateTime);
        public DateTime LastUpdated
        {
            get { return _lastUpdated; }
            set
            {
                SetProperty(ref _lastUpdated, value);
                OnPropertyChanged("LastUpdatedFormatted");
            }
        }

        [XmlIgnore]
        public string LastUpdatedFormatted
        {
            get 
            {
                return String.Format("{0}, {1}",
                                 _lastUpdated.ToString("m"),
                                 _lastUpdated.ToString("t"));
            }
        }

        private GasStationResult GetCheapestGasStation()
        {
            return GasStationResults.OrderBy(p => p.Price).FirstOrDefault();
        }

        [XmlIgnore]
        public double PriceAtCheapestGasStation
        {
            get
            {
                var cheapest = GetCheapestGasStation();
                return cheapest != null ? cheapest.Price : 0.0f;
            }
        }

        [XmlIgnore]
        public string PriceAtCheapestGasStationFormatted
        {
            get
            {
                var cheapest = GetCheapestGasStation();
                return cheapest != null ? cheapest.PriceFormatted : "Kein Preis";
            }
        }

        [XmlIgnore]
        public string InfoOnCheapestGasStation
        {
            get
            {
                var cheapest = GetCheapestGasStation();
                return cheapest != null ? cheapest.Name + ", " + cheapest.City : "";
            }
        }
    }
}
