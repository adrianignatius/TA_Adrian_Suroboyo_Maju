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
using Windows.UI.Popups;
using System.Threading;
using Newtonsoft.Json.Linq;

#if __ANDROID__
using Com.OneSignal;
using Com.OneSignal.Abstractions;
#endif

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

        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
#if __ANDROID__
            var status = await Permissions.RequestAsync<Permissions.LocationWhenInUse>();
            OneSignal.Current.AddTrigger("firstLogin", "true");
            var request = new GeolocationRequest(GeolocationAccuracy.Best,TimeSpan.FromSeconds(30));
            System.Threading.CancellationTokenSource cts = new System.Threading.CancellationTokenSource();
            var location = await Geolocation.GetLocationAsync(request,cts.Token);
            string queryParams = "?lat=" + location.Latitude + "&lng=" + location.Longitude;
            if (location != null)
            {
                string responseData = await new HttpObject().PutRequest("user/updateLocation/" + session.getUserLogin().id_user+queryParams,null,session.getTokenAuthorization());
                JObject json = JObject.Parse(responseData);
            }
#endif
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
                    this.Frame.Navigate(typeof(ProfilePage));
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
