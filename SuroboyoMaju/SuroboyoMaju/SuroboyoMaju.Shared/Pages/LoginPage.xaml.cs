using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SuroboyoMaju.Shared.Class;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using Xamarin.Essentials;
#if __ANDROID__
using Com.OneSignal.Abstractions;
using Com.OneSignal;
#endif


namespace SuroboyoMaju.Shared.Pages
{
    public sealed partial class LoginPage : Page
    {
        HttpObject httpObject;
        Session session;
        public LoginPage()
        {
            this.InitializeComponent();
            httpObject = new HttpObject();
            session = new Session();
        }

        public void goToRegister(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(RegisterPage));
        }

        private void validateInput(TextBox sender, TextBoxBeforeTextChangingEventArgs args)
        {
            args.Cancel = args.NewText.Any(c => !char.IsDigit(c));
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            this.Frame.BackStack.Clear();
        }

        public async void login(object sender, RoutedEventArgs e)
        {
            if (txtNoHandphone.Text.Length == 0 || txtPassword.Password.Length == 0)
            {
                var dialog = new MessageDialog("Silahkan isi Email dan Password terlebih dahulu!");
                await dialog.ShowAsync();
            }
            else
            {
                progressRing.IsActive = true;
                progressRing.Visibility = Visibility.Visible;
                var content = new FormUrlEncodedContent(new[]{
                    new KeyValuePair<string, string>("telpon_user", txtNoHandphone.Text),
                    new KeyValuePair<string, string>("password_user", txtPassword.Password),
                });
                string responseData = await httpObject.PostRequestWithUrlEncoded("checkLogin", content);
                JObject json = JObject.Parse(responseData);
                string statusCode = json["status"].ToString();
                if (statusCode == "200")
                {
                    string data = json["data"].ToString();
                    User userLogin = JsonConvert.DeserializeObject<User>(data);
                    await SecureStorage.SetAsync("jwt_token", json["token"].ToString());
                    session.setTokenAuthorization(json["token"].ToString());
                    session.setUserLogin(userLogin);
                    if (userLogin.status_aktif_user == 0)
                    {
                        progressRing.Visibility = Visibility.Visible;
                        progressRing.IsActive = false;
                        var message = new MessageDialog("Akun anda telah diban oleh admin, silahkan menghubungi admin untuk mengaktifkan kembali akun anda");
                        await message.ShowAsync();
                    }
                    else if (userLogin.status_aktif_user == 99)
                    {
                        this.Frame.Navigate(typeof(VerifyOtpPage));
                    }
                    else
                    {
#if __ANDROID__
                        OneSignal.Current.SendTags(new Dictionary<string, string>() { { "no_handphone", userLogin.telpon_user }, { "tipe_user", userLogin.status_user.ToString() } });
#endif
                        if (userLogin.status_user == 2)
                        {
                            this.Frame.Navigate(typeof(HomeNavigationPageKepalaKeamanan));
                        }
                        else
                        {
                            this.Frame.Navigate(typeof(HomeNavigationPage));
                        }
                    }
                }
                else
                {
                    progressRing.Visibility = Visibility.Visible;
                    progressRing.IsActive = false;
                    var dialog = new MessageDialog(json["message"].ToString());
                    await dialog.ShowAsync();
                }
            }
        }
    }
}
