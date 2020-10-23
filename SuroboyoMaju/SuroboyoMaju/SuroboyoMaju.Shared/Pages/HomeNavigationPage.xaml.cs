using SuroboyoMaju.Shared.Class;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Xamarin.Essentials;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace SuroboyoMaju.Shared.Pages
{
    public sealed partial class HomeNavigationPage : Page
    {
        Session session;
        public HomeNavigationPage()
        {
            this.InitializeComponent();
            session = new Session();
        }

        private void NavView_Loaded(object sender, RoutedEventArgs e)
        {

            foreach (NavigationViewItemBase item in NavView.MenuItems)
            {
                if (item is NavigationViewItem && item.Tag.ToString() == "home")
                {
                    NavView.SelectedItem = item;
                    NavView_Navigate(item as NavigationViewItem);
                    break;
                }
            }
        }

        private void NavView_ItemInvoked(NavigationView sender, NavigationViewItemInvokedEventArgs args)
        {
            if (args.IsSettingsInvoked)
            {
                ContentFrame.Navigate(typeof(LoginPage));
            }
            else
            {
                var item = sender.MenuItems.OfType<NavigationViewItem>().First(x => (string)x.Content == (string)args.InvokedItem);
                NavView_Navigate(item as NavigationViewItem);
            }
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            session.setHomeNavigationPageInstance(this);
        }

        private void NavView_Navigate(NavigationViewItem item)
        {
            switch (item.Tag)
            {
                case "home":
                    ContentFrame.Navigate(typeof(HomePage));
                    break;

                case "MakeLostFoundReportPage":
                    this.Frame.Navigate(typeof(MakeLostFoundReportPage));
                    break;

                case "MakeCrimeReportPage":
                    this.Frame.Navigate(typeof(MakeCrimeReportPage));
                    break;

                case "SignOut":
                    SecureStorage.Remove("jwt_token");
                    this.Frame.BackStack.Clear();
                    this.Frame.Navigate(typeof(LoginPage));
                    break;

                case "PoliceStationPage":
                    this.Frame.Navigate(typeof(PoliceStationListPage));
                    break;

                case "ProfilePage":
                    ContentFrame.Navigate(typeof(ProfilePage));
                    break;

                case "chatPage":
                    this.Frame.Navigate(typeof(ChatListPage));
                    break;

                case "contactPage":
                    ContentFrame.Navigate(typeof(ContactPage));
                    break;

                case "historyPage":
                    this.Frame.Navigate(typeof(HistoryPage));
                    break;
            }
        }
    }
}
