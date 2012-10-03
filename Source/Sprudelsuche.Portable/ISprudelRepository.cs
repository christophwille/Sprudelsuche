using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sprudelsuche.Portable.Model;

namespace Sprudelsuche.Portable
{
    public interface ISprudelRepository
    {
        Task<List<GasQueryResult>> LoadResultsAsync();
        Task<GasQueryResult> LoadResultAsync(string uniqueId);
        Task<bool> SaveResultsAsync(List<GasQueryResult> ergebnisse);
        Task<GasQueryRefreshResult> RefreshAsync();

        Task<bool> AddAsync(GasQuery parameters);
        Task<bool> AddResultAsync(GasQueryResult result);
    }
}
