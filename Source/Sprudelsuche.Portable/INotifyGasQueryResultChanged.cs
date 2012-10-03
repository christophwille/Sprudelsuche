using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sprudelsuche.Portable.Model;

namespace Sprudelsuche.Portable
{
    public interface INotifyGasQueryResultChanged
    {
        Task NotifyGasQueryResultChanged(GasQueryResult result);
    }
}
