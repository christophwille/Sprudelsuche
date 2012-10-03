using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using Sprudelsuche.Common;

namespace Sprudelsuche.ViewModels
{
    public interface IViewModelAwarePage<T>
        where T : ViewModelBase
    {
        T ViewModel { get; set; }
    }
}
