using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

namespace Sprudelsuche
{
    public sealed partial class ErrorWarningBarControl : UserControl
    {
        public ErrorWarningBarControl()
        {
            this.InitializeComponent();
        }

        private Popup _parentPopup = null;

        public ErrorWarningBarControl(Popup parentPopup, string description) : this()
        {
            if (null == parentPopup)
                throw new ArgumentNullException();

            _parentPopup = parentPopup;

            DescriptionTextblock.Text = description;
        }

        private void Dismiss_OnClick(object sender, RoutedEventArgs e)
        {
            _parentPopup.IsOpen = false;
        }
    }
}
