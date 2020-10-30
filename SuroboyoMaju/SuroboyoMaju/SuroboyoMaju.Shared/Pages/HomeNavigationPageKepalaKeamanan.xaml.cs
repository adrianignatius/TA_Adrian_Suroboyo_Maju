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
using Windows.UI.Xaml.Navigation;
using Xamarin.Essentials;

namespace SuroboyoMaju.Shared.Pages
{
    public sealed partial class HomeNavigationPageKepalaKeamanan : Page
    {
        Session session;
        public HomeNavigationPageKepalaKeamanan()
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
            base.OnNavigatedTo(e);
            session.setHomeNavigationPageKepalaKeamananInstance(this);
        }

        private void NavView_Navigate(NavigationViewItem item)
        {
            switch (item.Tag)
            {
                case "home":
                    ContentFrame.Navigate(typeof(HomePage));
                    break;
                case "AllLostFoundReportPage":
                    this.Frame.Navigate(typeof(AllLostFoundReportPage));
                    break;
                case "AllCrimeReportPage":
                    this.Frame.Navigate(typeof(AllCrimeReportPage));
                    break;
                case "AreaReportPage":
                    this.Frame.Navigate(typeof(LaporanKepalaKeamananPage));
                    break;
                case "contactPage":
                    ContentFrame.Navigate(typeof(ContactPage));
                    break;
                case "chatPage":
                    this.Frame.Navigate(typeof(ChatListPage));
                    break;
                case "SignOut":
                    this.Frame.Navigate(typeof(LoginPage));
                    SecureStorage.Remove("jwt_token");
                    break;
            }
        }
    }
}
