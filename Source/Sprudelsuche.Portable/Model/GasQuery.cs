using System;
using System.Globalization;
using System.Xml.Serialization;

namespace Sprudelsuche.Portable.Model
{
    public class GasQuery : ModelBase
    {
        public GasQuery()
        {
        }

        public GasQuery(string uniqueId, FuelTypeEnum fuelType)
        {
            UniqueId = uniqueId;
            _fuelType = fuelType;
        }

        public GasQuery(string uniqueId, FuelTypeEnum fuelType, double long1, double lat1, double long2, double lat2)
        {
            UniqueId = uniqueId;
            _fuelType = fuelType;

            _long1 = long1;
            _lat1 = lat1;
            _long2 = long2;
            _lat2 = lat2;
        }

        public GasQuery(GasQuery parameterOriginal)
        {
            UniqueId = parameterOriginal.UniqueId;
            _fuelType = parameterOriginal.FuelType;

            _long1 = parameterOriginal.Longitude1;
            _lat1 = parameterOriginal.Latitude1;
            _long2 = parameterOriginal.Longitude2;
            _lat2 = parameterOriginal.Latitude2;

            _showClosed = parameterOriginal.ShowClosedGasStations;

            Name = parameterOriginal.Name;
            _geocodeLatitude = parameterOriginal.GeocodeLatitude;
            _geocodeLongitude = parameterOriginal.GeocodeLongitude;
        }

        private FuelTypeEnum _fuelType = FuelTypeEnum.Diesel;
        public FuelTypeEnum FuelType
        {
            get { return _fuelType; }
            set { SetProperty(ref _fuelType, value); }
        }

        [XmlIgnore]
        public string FuelTypeDisplayName
        { 
            get 
            { 
                return Enum.GetName(typeof (FuelTypeEnum), this.FuelType); 
            }
        }

        [XmlIgnore]
        public string DetailTitle
        {
            get { return this.FuelTypeDisplayName + ": " + this.Name; }
        }

        private bool _showClosed = false;
        public bool ShowClosedGasStations
        {
            get { return _showClosed; }
            set { SetProperty(ref _showClosed, value); }
        }

        private double _long1 = 0.0f;
        public double Longitude1
        {
            get { return _long1; }
            set { SetProperty(ref _long1, value); }
        }

        private double _lat1 = 0.0f;
        public double Latitude1
        {
            get { return _lat1; }
            set { SetProperty(ref _lat1, value); }
        }

        private double _long2 = 0.0f;
        public double Longitude2
        {
            get { return _long2; }
            set { SetProperty(ref _long2, value); }
        }

        private double _lat2 = 0.0f;
        public double Latitude2
        {
            get { return _lat2; }
            set { SetProperty(ref _lat2, value); }
        }

        public string ToPostData()
        {
            // Querying for closed gas stations is currently not supported
            string postString = 
                String.Format("data=[\"\",\"{0}\",{1},{2},{3},{4}]", 
                this.FuelType.ToSpritpreisrechnerString(), 
                Longitude1.ToString(CultureInfo.InvariantCulture),
                Latitude1.ToString(CultureInfo.InvariantCulture),
                Longitude2.ToString(CultureInfo.InvariantCulture),
                Latitude2.ToString(CultureInfo.InvariantCulture));

            return postString;
        }

        // Part of geocoded center of the map (Name refers to the selected geocoded location)
        private double _geocodeLongitude = 0.0f;
        public double GeocodeLongitude
        {
            get { return _geocodeLongitude; }
            set { SetProperty(ref _geocodeLongitude, value); }
        }

        // Part of geocoded center of the map (Name refers to the selected geocoded location)
        private double _geocodeLatitude = 0.0f;
        public double GeocodeLatitude
        {
            get { return _geocodeLatitude; }
            set { SetProperty(ref _geocodeLatitude, value); }
        }
    }
}
