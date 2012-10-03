using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sprudelsuche.Portable.Model;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Markup;
using Windows.UI.Xaml.Media;

namespace Sprudelsuche.Assets
{
    public class FuelTypeBrushConverter : IValueConverter
    {
        private ResourceDictionary _resourceDictionary = new ResourceDictionary();
        public ResourceDictionary ResourceDictionary
        {
            get { return _resourceDictionary; }
            set
            {
                _resourceDictionary = value;
            }
        }

        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (null == value) return null;
            if (!(value is FuelTypeEnum)) return null;

            var art = (FuelTypeEnum)value;

            string resourceKey = Enum.GetName(typeof(FuelTypeEnum), art) + "Brush";

            return ResourceDictionary[resourceKey];
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
