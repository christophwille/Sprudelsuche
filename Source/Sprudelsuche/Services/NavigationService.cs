using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;

namespace Sprudelsuche.Services
{
    // Should be rewritten w. IoC along the lines of http://mikaelkoskinen.net/post/winrt-mvvm-navigation-example.aspx
    public static class NavigationService
    {
        private static Frame _frame;

        public static void Initialize(Frame frame)
        {
            _frame = frame;
        }

        public static bool Navigate<T>(object parameter = null)
        {
            return _frame.Navigate(typeof(T), parameter);
        }
    }
}
