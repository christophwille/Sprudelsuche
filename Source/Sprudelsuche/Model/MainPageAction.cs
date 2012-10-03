using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SprudelSuche.ThirdParty.U2U;

namespace Sprudelsuche.Model
{
    public class MainPageAction : IResizable
    {
        public const int GridViewItemWidth = 150;

        public string Name { get; set; }
        public string Description { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }

        public int ItemWidth
        {
            get { return Width * GridViewItemWidth; }
        }

        public bool IsActionDieselAtLocation()
        {
            return 0 == String.Compare(this.Name, ActionDieselAtLocation, StringComparison.OrdinalIgnoreCase);
        }

        public bool IsActionSuperAtLocation()
        {
            return 0 == String.Compare(this.Name, ActionSuperAtLocation, StringComparison.OrdinalIgnoreCase);
        }

        public const string ActionDieselAtLocation = "Diesel";
        public const string ActionSuperAtLocation = "Super";
        public const string ActionManualLocation = "Manuelle Ortsauswahl";

        public static List<MainPageAction> GenerateMainPageActions()
        {
            return new List<MainPageAction>()
                        {
                            new MainPageAction() {Name = ActionDieselAtLocation, Description="an meinem Standort", Height = 1, Width = 1},
                            new MainPageAction() {Name = ActionSuperAtLocation, Description="an meinem Standort",Height = 1, Width = 1},
                            new MainPageAction() {Name = ActionManualLocation, Description="",Height = 1, Width = 2}
                        };
        }
    }
}
