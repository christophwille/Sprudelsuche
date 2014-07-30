﻿using Windows.ApplicationModel;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Caliburn.Micro;
using System;
using System.Collections.Generic;
using Windows.ApplicationModel.Activation;
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

            container
                .PerRequest<MainViewModel>()
                .PerRequest<CurrentGasPricesViewModel>(); 
        }

        protected override void PrepareViewFirst(Frame rootFrame)
        {
            container.RegisterNavigationService(this.RootFrame);
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

            ////var resumed = false;

            ////if (args.PreviousExecutionState == ApplicationExecutionState.Terminated)
            ////{
            ////    resumed = navigationService.ResumeState();
            ////}

            ////if (!resumed)
            ////    DisplayRootViewFor<MainViewModel>();


            // THIS CRASHES WITH AN ACCESS VIOLATION (AV) - no idea why
            // DisplayRootView<MainView>();

            // AND THIS DOESN'T WORK PROPERLY WITH NAVIGATION
            DisplayRootViewFor<MainViewModel>();
        }

        protected override void OnSuspending(object sender, SuspendingEventArgs e)
        {
            navigationService.SuspendState();
        }
    }
}