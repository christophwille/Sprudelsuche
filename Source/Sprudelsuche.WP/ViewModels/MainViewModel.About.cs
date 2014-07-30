using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Store;

namespace Sprudelsuche.WP.ViewModels
{
    public partial class MainViewModel
    {
        void InitializeAbout()
        {
            VersionText = GetAppVersion();

            GitHubUrl = new Uri("https://github.com/christophwille/Sprudelsuche");
            PrivacyPolicyUrl = new Uri("https://github.com/christophwille/Sprudelsuche/wiki/Datenschutzerkl%C3%A4rung-%28Privacy-Statement%29");
        }

        public string VersionText { get; set; }

        private string GetAppVersion()
        {
            PackageVersion version = Package.Current.Id.Version;
            return string.Format("{0}.{1}.{2}.{3}", version.Major, version.Minor, version.Build, version.Revision);
        }

        public Uri GitHubUrl { get; set; }
        public Uri PrivacyPolicyUrl { get; set; }

        public async void Review()
        {
            await Windows.System.Launcher.LaunchUriAsync(new Uri("ms-windows-store:reviewapp?appid=" + CurrentApp.AppId));
        }
    }
}
