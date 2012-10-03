using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using Sprudelsuche.Portable;

namespace Sprudelsuche.ViewModels
{
    public class SprudelViewModelBase : ViewModelBase
    {
        public Func<ISprudelRepository> CreateSprudelRepository { get; set; }

        public SprudelViewModelBase()
        {
            CreateSprudelRepository = () => new Sprudelsuche.WinRT.SprudelRepository();
        }

        public string BingMapsCredentials
        {
            get { return BingMaps.Credentials; }
        }
    }
}
