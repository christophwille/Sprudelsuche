using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using Newtonsoft.Json;

namespace Sprudelsuche.WP.Services
{
    public class DefaultFavoritesRepository : IFavoritesRepository
    {
        private const string FavoritesFileName = "favorites.json";
        private List<Favorite> _loadedFavorites = null;

        private StorageFolder GetFavoritesFolder()
        {
            return Windows.Storage.ApplicationData.Current.LocalFolder;
        }

        private async Task<string> ReadFavoritesFileAsync()
        {
            try
            {
                var file = await GetFavoritesFolder()
                    .GetFileAsync(FavoritesFileName)
                    .AsTask().ConfigureAwait(false);

                if (file == null)
                    return "";

                return await FileIO.ReadTextAsync(file).AsTask().ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
            }

            return "";
        }

        private async Task WriteFavoritesFileAsync(string favs)
        {
            try
            {
                var file = await GetFavoritesFolder()
                    .CreateFileAsync(FavoritesFileName, CreationCollisionOption.ReplaceExisting)
                    .AsTask().ConfigureAwait(false);

                await FileIO.WriteTextAsync(file, favs).AsTask().ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
            }
        }

        private async Task SaveFavoritesAsync(List<Favorite> favorites)
        {
            string content = JsonConvert.SerializeObject(favorites);
            await WriteFavoritesFileAsync(content).ConfigureAwait(false);
        }

        public async Task<List<Favorite>> LoadAsync()
        {
            if (null != _loadedFavorites)
                return _loadedFavorites;

            string favoritesFileContents = await ReadFavoritesFileAsync();

            if (!String.IsNullOrWhiteSpace(favoritesFileContents))
            {
                var favorites = JsonConvert.DeserializeObject<List<Favorite>>(favoritesFileContents);
                if (favorites != null && favorites.Count > 0) _loadedFavorites = favorites;
            }

            return _loadedFavorites;
        }

        // A very simple search that looks for name only
        private Favorite FindFavorite(List<Favorite> favorites, Favorite lookingFor)
        {
            return favorites
                .FirstOrDefault(f => f.LocationName == lookingFor.LocationName);
        }

        public async Task<List<Favorite>> AddAsync(Favorite toAdd)
        {
            var favorites = await LoadAsync().ConfigureAwait(false);

            if (null != favorites)
            {
                if (null != FindFavorite(favorites, toAdd)) return favorites; // Do not add twice to the list
            }
            else
            {
                favorites = new List<Favorite>();
            }

            favorites.Add(toAdd);
            await SaveFavoritesAsync(favorites).ConfigureAwait(false);

            _loadedFavorites = favorites;
            return _loadedFavorites;
        }


        public async Task<List<Favorite>> RemoveAsync(Favorite toRemove)
        {
            var favorites = await LoadAsync().ConfigureAwait(false);

            if (null == favorites) return null;  // that should never happen actually

            var found = FindFavorite(favorites, toRemove);
            if (null == found) return favorites;    // not in the list? nothing to do

            favorites.Remove(found);
            await SaveFavoritesAsync(favorites).ConfigureAwait(false);

            _loadedFavorites = favorites;
            return _loadedFavorites;
        }
    }
}
