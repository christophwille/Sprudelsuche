using Windows.ApplicationModel;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Caliburn.Micro;
using System;
using System.Collections.Generic;
using Windows.ApplicationModel.Activation;
using Sprudelsuche.WP.Services;
using Sprudelsuche.WP.ViewModels;
using Sprudelsuche.WP.Views;

namespace Sprudelsuche.WP
{
    public sealed partial class App
    {
        private WinRTContainer container;
        private INavigationService navigationService;

        public App()
        {
            InitializeComponent();
        }

        protected override void Configure()
        {
            LogManager.GetLog = t => new DebugLog(t);

            container = new WinRTContainer();
            container.RegisterWinRTServices();

            container.RegisterInstance(typeof(IFavoritesRepository), null, new DefaultFavoritesRepository());

            container.RegisterPerRequest(typeof(IMessageService), null, typeof(DefaultMessageService));

            container
                .PerRequest<MainViewModel>()
                .PerRequest<CurrentGasPricesViewModel>(); 
        }

        protected override void PrepareViewFirst(Frame rootFrame)
        {
            navigationService = container.RegisterNavigationService(rootFrame);
        }

        protected override object GetInstance(Type service, string key)
        {
            var instance = container.GetInstance(service, key);
            if (instance != null)
                return instance;
            throw new Exception("Could not locate any instances.");
        }

        protected override IEnumerable<object> GetAllInstances(Type service)
        {
            return container.GetAllInstances(service);
        }

        protected override void BuildUp(object instance)
        {
            container.BuildUp(instance);
        }

        protected override void OnLaunched(LaunchActivatedEventArgs args)
        {
            Initialize();

            PrepareViewFirst();

            var resumed = false;

            if (args.PreviousExecutionState == ApplicationExecutionState.Terminated)
            {
                resumed = navigationService.ResumeState();
            }

            if (!resumed)
                DisplayRootView<MainView>();
        }

        protected override void OnSuspending(object sender, SuspendingEventArgs e)
        {
            navigationService.SuspendState();
        }
    }
}
