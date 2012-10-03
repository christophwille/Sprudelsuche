using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;

namespace Sprudelsuche.Services
{
    public static class ResourceService
    {
        public static T Get<T>(string resourceName)
            where T : class
        {
            return Application.Current.Resources[resourceName] as T;
        }
    }
}
