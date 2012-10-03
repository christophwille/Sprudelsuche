using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NotificationsExtensions.TileContent;
using Sprudelsuche.Portable;
using Sprudelsuche.Portable.Model;
using Windows.UI.Notifications;
using Windows.UI.StartScreen;

namespace Sprudelsuche.WinRT
{
    public class NotifyTileOfGasQueryResultChanged : INotifyGasQueryResultChanged
    {
        public async Task NotifyGasQueryResultChanged(GasQueryResult currentResult)
        {
            string tileId = currentResult.UniqueId;
            bool isCurrentlyPinned = SecondaryTile.Exists(tileId);

            if (isCurrentlyPinned)
            {
                string details = String.Format("{0} ({1})", currentResult.Name, currentResult.LastUpdatedFormatted);

                // http://msdn.microsoft.com/en-us/library/windows/apps/hh761491.aspx
                var wideTile = TileContentFactory.CreateTileWideText09();
                wideTile.TextHeading.Text = currentResult.PriceAtCheapestGasStation.ToString();
                wideTile.TextBodyWrap.Text = details;

                var squareTile = TileContentFactory.CreateTileSquareText02();
                squareTile.TextHeading.Text = currentResult.PriceAtCheapestGasStation.ToString();
                squareTile.TextBodyWrap.Text = details;

                wideTile.SquareContent = squareTile;

                TileUpdateManager.CreateTileUpdaterForSecondaryTile(tileId)
                    .Update(wideTile.CreateNotification());
            }
        }
    }
}
