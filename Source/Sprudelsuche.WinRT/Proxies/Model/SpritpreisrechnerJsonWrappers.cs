using System.Collections.Generic;

//
// Generated with: http://json2csharp.com/
//
namespace Sprudelsuche.WinRT.Proxies.Model
{
    internal class ErrorItem
    {
        public string field { get; set; }
        public string error { get; set; }
        public string msgText { get; set; }
    }

    internal class SpritPrice
    {
        public string amount { get; set; }
        public string datAnounce { get; set; }
        public List<ErrorItem> errorItems { get; set; }
        public int errorCode { get; set; }
        public object datValid { get; set; }
        public string spritId { get; set; }
    }

    internal class Day
    {
        public string dayLabel { get; set; }
        public int order { get; set; }
        public List<ErrorItem> errorItems { get; set; }
        public int errorCode { get; set; }
        public string day { get; set; }
    }

    internal class OpeningHour
    {
        public string beginn { get; set; }
        public Day day { get; set; }
        public string end { get; set; }
    }

    internal class RootObject
    {
        public bool kredit { get; set; }
        public bool self { get; set; }
        public List<SpritPrice> spritPrice { get; set; }
        public bool automat { get; set; }
        public string city { get; set; }
        public bool open { get; set; }
        public double distance { get; set; }
        public string postalCode { get; set; }
        public List<ErrorItem> errorItems { get; set; }
        public bool priceSearchDisabled { get; set; }
        public string longitude { get; set; }
        public string payMethod { get; set; }
        public string mail { get; set; }
        public string gasStationName { get; set; }
        public string fax { get; set; }
        public string clubCard { get; set; }
        public List<OpeningHour> openingHours { get; set; }
        public string access { get; set; }
        public string url { get; set; }
        public string serviceText { get; set; }
        public bool maestro { get; set; }
        public bool companionship { get; set; }
        public string address { get; set; }
        public bool club { get; set; }
        public bool service { get; set; }
        public int errorCode { get; set; }
        public string latitude { get; set; }
        public bool bar { get; set; }
        public string telephone { get; set; }
    }
}
