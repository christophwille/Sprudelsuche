using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Callisto.Controls;
using Sprudelsuche.Common;
using Sprudelsuche.Services;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.ApplicationSettings;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Application template is documented at http://go.microsoft.com/fwlink/?LinkId=234227

namespace Sprudelsuche
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    sealed partial class App : Application
    {
        /// <summary>
        /// Initializes the singleton application object.  This is the first line of authored code
        /// executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
        public App()
        {
            this.InitializeComponent();
            this.Suspending += OnSuspending;
        }

        /// <summary>
        /// Invoked when the application is launched normally by the end user.  Other entry points
        /// will be used when the application is launched to open a specific file, to display
        /// search results, and so forth.
        /// </summary>
        /// <param name="args">Details about the launch request and process.</param>
        protected override async void OnLaunched(LaunchActivatedEventArgs args)
        {
            // Do not repeat app initialization when already running, just ensure that
            // the window is active
            if (args.PreviousExecutionState == ApplicationExecutionState.Running)
            {
                if (!String.IsNullOrEmpty(args.Arguments))
                    ((Frame)Window.Current.Content).Navigate(typeof(SprudelDetailPage), args.Arguments);

                Window.Current.Activate();
                return;
            }

            // Register handler for CommandsRequested events from the settings pane
            SettingsPane.GetForCurrentView().CommandsRequested += OnCommandsRequested;

            // Create a Frame to act navigation context and navigate to the first page
            var rootFrame = new Frame();
            NavigationService.Initialize(rootFrame);
            SuspensionManager.RegisterFrame(rootFrame, "AppFrame");

            // Secondary Tile Activation
            if (!String.IsNullOrEmpty(args.Arguments))
            {
                rootFrame.Navigate(typeof(SprudelDetailPage), args.Arguments);
                Window.Current.Content = rootFrame;
                Window.Current.Activate();
                return;
            }

            if (args.PreviousExecutionState == ApplicationExecutionState.Terminated)
            {
                await SuspensionManager.RestoreAsync();
            }

            if (rootFrame.Content == null)
            {
                if (!rootFrame.Navigate(typeof (MainPage)))
                {
                    throw new Exception("Failed to create initial page");
                }
            }
            // Place the frame in the current Window and ensure that it is active
            Window.Current.Content = rootFrame;
            Window.Current.Activate();
        }

        /// <summary>
        /// Invoked when application execution is being suspended.  Application state is saved
        /// without knowing whether the application will be terminated or resumed with the contents
        /// of memory still intact.
        /// </summary>
        /// <param name="sender">The source of the suspend request.</param>
        /// <param name="e">Details about the suspend request.</param>
        private async void OnSuspending(object sender, SuspendingEventArgs e)
        {
            var deferral = e.SuspendingOperation.GetDeferral();

            await SuspensionManager.SaveAsync();

            deferral.Complete();
        }

        void OnCommandsRequested(SettingsPane sender, SettingsPaneCommandsRequestedEventArgs args)
        {
            // Add an About command
            var about = new SettingsCommand("about", "Über", (handler) =>
            {
                var settings = new SettingsFlyout();
                settings.Content = new AboutUserControl();

                //var backgroundBrush = ResourceService.Get<SolidColorBrush>("AppBackgroundColorBrush");
                //settings.HeaderBrush = backgroundBrush;
                //settings.Background = backgroundBrush;
                settings.HeaderText = "Über";
                settings.IsOpen = true;
            });

            args.Request.ApplicationCommands.Add(about);

            // Add a Preferences command
            var preferences = new SettingsCommand("preferences", "Einstellungen", (handler) =>
            {
                var settings = new SettingsFlyout();
                settings.Content = new PreferencesUserControl();

                //var backgroundBrush = ResourceService.Get<SolidColorBrush>("AppBackgroundColorBrush");
                //settings.HeaderBrush = backgroundBrush;
                //settings.Background = backgroundBrush;
                settings.HeaderText = "Einstellungen";
                settings.IsOpen = true;
            });

            args.Request.ApplicationCommands.Add(preferences);
        }
    }
}
