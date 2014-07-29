using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using NotificationsExtensions.TileContent;
using Sprudelsuche.Model;
using Sprudelsuche.Portable.Model;
using Sprudelsuche.Services;
using Sprudelsuche.Portable.Proxies;
using Windows.ApplicationModel.Background;
using Windows.UI.Notifications;
using Windows.UI.Popups;
using Windows.UI.StartScreen;

namespace Sprudelsuche.ViewModels
{
    public class MainPageViewModel : SprudelViewModelBase
    {
        public MainPageViewModel()
            : base()
        {
            MainPageActions = MainPageAction.GenerateMainPageActions();

            GasQueryResults.CollectionChanged += (sender, e) => RaisePropertyChanged(AreQueryResultsAvailablePropertyName);
        }

        private readonly ObservableCollection<GasQueryResult> _results = new ObservableCollection<GasQueryResult>();
        public ObservableCollection<GasQueryResult> GasQueryResults
        {
            get { return _results; }
        }

        public async Task LoadDataAsync()
        {
            var repo = CreateSprudelRepository();
            var ergebnisse = await repo.LoadResultsAsync();


            if (!ergebnisse.Any())
            {
#if DEBUG && USE_SAMPLEDATA_IN_DEBUGMODE
                // DEMO values ONLY exist for local DEBUG builds
                ergebnisse = (new SampleDataSource()).SuchErgebnisse.ToList();
                var saveOk = await repo.SaveResultsAsync(ergebnisse);
#endif
            }

            foreach (var ergebnis in ergebnisse)
            {
                GasQueryResults.Add(ergebnis);
            }
        }

        public const string RefreshInProgressPropertyName = "RefreshInProgress";
        private bool _refreshInProgress = false;

        public bool RefreshInProgress
        {
            get
            {
                return _refreshInProgress;
            }
            set
            {
                Set(RefreshInProgressPropertyName, ref _refreshInProgress, value);
            }
        }

        private RelayCommand _refreshDataCommand;
        public RelayCommand RefreshDataCommand
        {
            get
            {
                return _refreshDataCommand
                    ?? (_refreshDataCommand = new RelayCommand(
                        async () => await RefreshDataAsync()));
            }
        }

        public async Task RefreshDataAsync()
        {
            RefreshInProgress = true;

            var repo = CreateSprudelRepository();
            var newInfos = await repo.RefreshAsync();

            RefreshInProgress = false;

            if (newInfos.Succeeded)
            {
                GasQueryResults.Clear();

                foreach (var current in newInfos.Results)
                {
                    GasQueryResults.Add(current);
                }
            }
            else
            {
                ErrorService.ShowLightDismissError("Refresh der Preise ist fehlgeschlagen");
            }
        }

        private RelayCommand _deleteSelectedItemCommand;
        public RelayCommand DeleteSelectedItemCommand
        {
            get
            {
                return _deleteSelectedItemCommand
                    ?? (_deleteSelectedItemCommand = new RelayCommand(
                        async () => await DeleteSelectedItemAsync(),
                        () => SelectedGasQueryResult != null));
            }
        }

        public async Task DeleteSelectedItemAsync()
        {
            // First, see if there is a secondary tile for what we are trying to delete right now
            string tileId = SelectedGasQueryResult.UniqueId;
            bool isCurrentlyPinned = SecondaryTile.Exists(tileId);

            if (isCurrentlyPinned)
            {
                var secondaryTile = new SecondaryTile(tileId);
                bool isUnpinned = await secondaryTile.RequestDeleteAsync();
            }

            // And now delete the item 
            GasQueryResults.Remove(SelectedGasQueryResult);

            bool saveOk = await CreateSprudelRepository().SaveResultsAsync(GasQueryResults.ToList());
        }

        public const string SelectedGasQueryResultPropertyName = "SelectedGasQueryResult";
        private GasQueryResult _selectedQueryResult = null;
        public GasQueryResult SelectedGasQueryResult
        {
            get { return _selectedQueryResult; }
            set
            {
                Set(SelectedGasQueryResultPropertyName, ref _selectedQueryResult, value);

                DeleteSelectedItemCommand.RaiseCanExecuteChanged();
                RaisePropertyChanged(IsGasQueryResultSelectedPropertyName);
            }
        }

        public const string IsGasQueryResultSelectedPropertyName = "IsGasQueryResultSelected";
        public bool IsGasQueryResultSelected
        {
            get { return _selectedQueryResult != null; }
        }

        public const string AreQueryResultsAvailablePropertyName = "AreQueryResultsAvailable";
        public bool AreQueryResultsAvailable
        {
            get { return _results.Count > 0; }
        }

        public const string MainPageActionsPropertyName = "MainPageActions";
        private List<MainPageAction> _mainpageActions = null;
        public List<MainPageAction> MainPageActions
        {
            get { return _mainpageActions; }
            set
            {
                Set(MainPageActionsPropertyName, ref _mainpageActions, value);
            }
        }
    }
}
