using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sprudelsuche.WP.Services
{
    public interface IFavoritesRepository
    {
        Task<List<Favorite>> LoadAsync();
        Task<List<Favorite>> AddAsync(Favorite toAdd);
        Task<List<Favorite>> RemoveAsync(Favorite toRemove);
    }
}
