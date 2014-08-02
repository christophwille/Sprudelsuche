using System;
using System.Xml.Serialization;

namespace Sprudelsuche.Portable.Model
{
    public class GasStationResult : ModelBase
    {
        private double _price = 0.0f;
        public double Price
        {
            get { return _price; }
            set { SetProperty(ref _price, value); }
        }

        private DateTime _dateReported;
        public DateTime DateReported
        {
            get { return _dateReported; }
            set { SetProperty(ref _dateReported, value); }
        }

        private double _lat = 0.0f;
        public double Latitude
        {
            get { return _lat; }
            set { SetProperty(ref _lat, value); }
        }

        private double _long = 0.0f;
        public double Longitude
        {
            get { return _long; }
            set { SetProperty(ref _long, value); }
        }

        private string _postalCode = string.Empty;
        public string PostalCode
        {
            get { return _postalCode; }
            set { SetProperty(ref _postalCode, value); }
        }

        private string _city = string.Empty;
        public string City
        {
            get { return _city; }
            set { SetProperty(ref _city, value); }
        }

        private string _street = string.Empty;
        public string Street
        {
            get { return _street; }
            set { SetProperty(ref _street, value); }
        }

        [XmlIgnore]
        public string Address
        {
            get
            {
                return String.Format("{0}, {1} {2}", Street, PostalCode, City);
            }
        }

        [XmlIgnore]
        public string PriceFormatted
        {
            get { return "€ " + Price.ToString(); }
        }
    }
}
