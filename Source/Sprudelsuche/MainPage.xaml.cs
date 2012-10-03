using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using Sprudelsuche.Portable.Model;
using Sprudelsuche.Services;
using Sprudelsuche.WinRT.Proxies;
using Sprudelsuche.ViewModels;
using Windows.ApplicationModel.Background;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Sprudelsuche.Model;

// The Items Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234233

namespace Sprudelsuche
{
    /// <summary>
    /// A page that displays a collection of item previews.  In the Split Application this page
    /// is used to display and select one of the available groups.
    /// </summary>
    public sealed partial class MainPage : Sprudelsuche.Common.LayoutAwarePage, IViewModelAwarePage<MainPageViewModel>
    {
        public MainPageViewModel ViewModel { get; set; }

        public MainPage()
        {
            this.InitializeComponent();

            if (!ViewModelBase.IsInDesignModeStatic)
            {
                ViewModel = new MainPageViewModel();
                DataContext = ViewModel;
            }

            var task = UpdateTaskManagementService.GetTaskRegistration();
            if (null != task)
                task.Completed += new BackgroundTaskCompletedEventHandler(OnCompleted);
        }

        /// <summary>
        /// Populates the page with content passed during navigation.  Any saved state is also
        /// provided when recreating a page from a prior session.
        /// </summary>
        /// <param name="navigationParameter">The parameter value passed to
        /// <see cref="Frame.Navigate(Type, Object)"/> when this page was initially requested.
        /// </param>
        /// <param name="pageState">A dictionary of state preserved by this page during an earlier
        /// session.  This will be null the first time a page is visited.</param>
        protected override void LoadState(Object navigationParameter, Dictionary<String, Object> pageState)
        {
            ManageAppBarOnSelection();

            Dispatcher.RunAsync(CoreDispatcherPriority.Normal, 
                async () =>
                          {
                              await ViewModel.LoadDataAsync();
                          });
        }

        private void OnCompleted(IBackgroundTaskRegistration task, BackgroundTaskCompletedEventArgs args)
        {
            Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
                () => ViewModel.LoadDataAsync());
        }

        private void ItemsControlsItemClick(object sender, ItemClickEventArgs e)
        {
            var itemId = ((GasQueryResult)e.ClickedItem).UniqueId;
            this.Frame.Navigate(typeof(SprudelDetailPage), itemId);
        }

        private void ItemsControlsSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ManageAppBarOnSelection();
        }

        private void ManageAppBarOnSelection()
        {
            if (this.itemGridView.SelectedItems.Count > 0)
            {
                this.BottomAppBar.IsSticky = true;
                this.BottomAppBar.IsOpen = true;
                this.Delete.IsEnabled = true;
            }
            else
            {
                this.BottomAppBar.IsOpen = false;
                this.BottomAppBar.IsSticky = false;
                this.Delete.IsEnabled = false;
            }
        }

        private void CommandsGridView_OnItemClick(object sender, ItemClickEventArgs e)
        {
            var action = (MainPageAction) e.ClickedItem;

            if (action.IsActionDieselAtLocation())
            {
                NavigateToDetailPage(FuelTypeEnum.Diesel);
                return;
            }

            if (action.IsActionSuperAtLocation())
            {
                NavigateToDetailPage(FuelTypeEnum.Super);
                return;
            }

            this.Frame.Navigate(typeof(AddSprudelSuche));
        }

        private void NavigateToDetailPage(FuelTypeEnum fuelType)
        {
            this.Frame.Navigate(typeof(SprudelDetailPage), Constants.QuickSearchPrefix + fuelType.ToString());
        }
    }
}
