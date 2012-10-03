using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls.Primitives;

namespace Sprudelsuche.Services
{
    public static class ErrorService
    {
        private const double HalfWidthOfErrorControl = 400.0;

        public static void ShowLightDismissError(string description)
        {
            var _popup = new Popup();

            var errorbar = new ErrorWarningBarControl(_popup, description);
            var bounds = Window.Current.Bounds;
            errorbar.MinWidth = bounds.Width;

            _popup.Child = errorbar;
            _popup.VerticalOffset = 0.0;
            _popup.HorizontalOffset = 0.0f;
            _popup.IsOpen = true;
            _popup.IsLightDismissEnabled = true;
        }
    }
}
