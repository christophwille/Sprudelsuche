using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NotificationsExtensions.BadgeContent;
using NotificationsExtensions.TileContent;
using Sprudelsuche.Portable;
using Sprudelsuche.Portable.Model;
using SprudelSuche.ThirdParty.XAMLMetroAppIsolatedStorageHelper;
using Sprudelsuche.WinRT.Proxies;
using Windows.UI.Notifications;
using Windows.UI.StartScreen;

namespace Sprudelsuche.WinRT
{
    public class SprudelRepository : ISprudelRepository
    {
        public Func<IGasPriceInfoProxy> CreateGasPriceInfoProxy { get; set; }
        public Func<INotifyGasQueryResultChanged> CreateResultChangedNotification { get; set; } 

        public SprudelRepository()
        {
            CreateGasPriceInfoProxy = () => new SpritpreisrechnerProxy();
            CreateResultChangedNotification = () => new NotifyTileOfGasQueryResultChanged();
        }

        private const string StorageFileName = "sprudeldb";

        public async Task<List<GasQueryResult>> LoadResultsAsync()
        {
            var objectStorageHelper = new StorageHelper<List<GasQueryResult>>(StorageType.Local);
            List<GasQueryResult> ergebnisse = await objectStorageHelper.LoadAsync(StorageFileName);

            if (null == ergebnisse)
                ergebnisse = new List<GasQueryResult>();

            return ergebnisse;
        }

        public async Task<GasQueryResult> LoadResultAsync(string uniqueId)
        {
            var allErgebnisse = await LoadResultsAsync();
            return allErgebnisse.FirstOrDefault(se => se.UniqueId == uniqueId);
        }

        public async Task<bool> SaveResultsAsync(List<GasQueryResult> ergebnisse)
        {
            var objectStorageHelper = new StorageHelper<List<GasQueryResult>>(StorageType.Local);
            bool result = await objectStorageHelper.SaveAsync(ergebnisse, StorageFileName);
            return result;
        }

        public async Task<GasQueryRefreshResult> RefreshAsync()
        {
            bool bExceptionInExecution = false;

            try
            {
                // Show activitiy
                var badgeContent = new BadgeGlyphNotificationContent(GlyphValue.Activity);
                BadgeUpdateManager.CreateBadgeUpdaterForApplication().Update(badgeContent.CreateNotification());

                // TODO: Work with roaming parameters as well as local results
                var existingResults = await LoadResultsAsync();

                var taskList = new List<Task<GasQueryDownloadResult>>();
                foreach (GasQueryResult current in existingResults)
                {
                    var copyForClosure = current;

                    var task = new Task<GasQueryDownloadResult>(() => CreateGasPriceInfoProxy().Download(copyForClosure).Result);
                    taskList.Add(task);
                    task.Start();
                }

                await Task.WhenAll(taskList);

                List<GasQueryResult> returnedResults = new List<GasQueryResult>();
                var notify = CreateResultChangedNotification();
                
                foreach (var current in taskList)
                {
                    var downloadResult = current.Result;
                    GasQueryResult currentResult = downloadResult.Result;

                    if (downloadResult.Succeeded)
                    {
                        returnedResults.Add(currentResult);

                        await notify.NotifyGasQueryResultChanged(currentResult);
                    }
                    else
                    {
                        // Keep the old information around instead
                        var oldToReuse = existingResults.Single(r => r.UniqueId == currentResult.UniqueId);
                        returnedResults.Add(oldToReuse);
                    }
                }

                bool saveOk = await SaveResultsAsync(returnedResults);

                return new GasQueryRefreshResult()
                           {
                               Succeeded = true,
                               Results = returnedResults
                           };
            }
            catch (Exception)
            {
                bExceptionInExecution = false;
            }
            finally
            {
                BadgeUpdateManager.CreateBadgeUpdaterForApplication().Clear();
            }

            return new GasQueryRefreshResult()
                       {
                           Succeeded = false,
                           Results = null
                       };
        }

        public async Task<bool> AddAsync(GasQuery parameters)
        {
            try
            {
                var proxy = CreateGasPriceInfoProxy();
                var ergebnis = await proxy.Download(parameters);

                if (ergebnis.Succeeded)
                {
                    bool saveOk = await AddResultAsync(ergebnis.Result);
                    return saveOk;
                }
            }
            catch (Exception)
            {
            }

            return false;
        }

        public async Task<bool> AddResultAsync(GasQueryResult result)
        {
            try
            {
                var existingErgebnisse = await LoadResultsAsync();
                existingErgebnisse.Add(result);
                bool saveOk = await SaveResultsAsync(existingErgebnisse);

                return saveOk;
            }
            catch (Exception)
            {
                return false;
            }
        }


        // Static Helpers for Testing ONLY
        public static async Task ResetStorageAsync()
        {
            var objectStorageHelper = new StorageHelper<List<GasQueryResult>>(StorageType.Local);
            objectStorageHelper.DeleteAsync(StorageFileName);
        }
    }
}
