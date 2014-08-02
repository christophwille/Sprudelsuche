using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Sprudelsuche.Portable.Model;
using Sprudelsuche.WP.Models;

namespace Sprudelsuche.WP.ViewModels
{
    // Make sure the design-time vm matches the runtime vm for full Blend fidelity in creating the bindings
    public interface IMainViewModelBindings
    {
        bool Loading { get; set; }
        string SearchText { get; set; }
        bool DieselSelected { get; set; }
        bool SuperSelected { get; set; }
        ObservableCollection<GeocodeResult> Results { get; set; }
        ObservableCollection<Favorite> Favorites { get; set; }
        ICommand RemoveFavoriteCommand { get; }

        // About
        string VersionText { get; set; }
        Uri GitHubUrl { get; set; }
        Uri PrivacyPolicyUrl { get; set; }
    }
}
