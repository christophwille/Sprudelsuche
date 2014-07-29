using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;

namespace Sprudelsuche.Services
{
    public static class MessageService
    {
        // Unused and untested 
        public static void ShowMessage(string message, UIElement placementTarget, PlacementMode placementMode=PlacementMode.Top)
        {
            var f = new Callisto.Controls.Flyout();

            var b = new Border()
                           {
                               Width = 300,
                               Height = 125
                           };

            TextBlock tb = new TextBlock();
            tb.HorizontalAlignment = HorizontalAlignment.Center;
            tb.VerticalAlignment = VerticalAlignment.Center;
            tb.TextWrapping = TextWrapping.Wrap;
            tb.FontSize = 12f;
            tb.Text = message;

            b.Child = tb;

            f.Content = b;

            f.Placement = placementMode;
            f.PlacementTarget = placementTarget;

            f.IsOpen = true;
        }
    }
}
