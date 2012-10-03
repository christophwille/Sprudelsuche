using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Sprudelsuche.Services;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

namespace Sprudelsuche
{
    public sealed partial class PreferencesUserControl : UserControl
    {
        public PreferencesUserControl()
        {
            this.InitializeComponent();

            AutomaticBackgroundTask.IsOn = UpdateTaskManagementService.IsTaskRegistered();
        }

        private async void AutomaticBackgroundTask_Toggled(object sender, RoutedEventArgs e)
        {
            bool bDoRegisterTask = AutomaticBackgroundTask.IsOn;

            if (bDoRegisterTask)
            {
                var task = await UpdateTaskManagementService.Register();

                if (null == task)
                {
                    AutomaticBackgroundTask.IsOn = false;

                    var msg = new MessageDialog("Sie haben die Zustimmung verweigert, eine Aktivierung dieses Features ist in der Applikation nicht möglich", "Fehler");
                    await msg.ShowAsync();
                }
            }
            else
            {
                UpdateTaskManagementService.Unregister();
            }
        }
    }
}
