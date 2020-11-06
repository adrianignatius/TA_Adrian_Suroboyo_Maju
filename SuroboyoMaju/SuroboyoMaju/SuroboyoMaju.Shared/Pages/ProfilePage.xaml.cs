﻿using Newtonsoft.Json.Linq;
using SuroboyoMaju.Shared.Class;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace SuroboyoMaju.Shared.Pages
{
    public sealed partial class ProfilePage : Page
    {
        Session session;
        HttpObject httpObject;
        User userLogin;
        DispatcherTimer dispatcherTimer;
        string lat = null, lng = null, lokasiUser = null;
        ObservableCollection<AutocompleteAddress> listAutoCompleteAddress = new ObservableCollection<AutocompleteAddress>();
        int tick = 0;
        bool isChosen = false;
        public ProfilePage()
        {
            this.InitializeComponent();
            session = new Session();
            httpObject = new HttpObject();
            dispatcherTimer = new DispatcherTimer();
            dispatcherTimer.Interval = new TimeSpan(0, 0, 0, 0, 200);
            dispatcherTimer.Tick += DispatcherTimer_Tick;
            userLogin = session.getUserLogin();
        }

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.GoBack();
        }

        private void DispatcherTimer_Tick(object sender, object e)
        {
            tick++;
            if (tick == 2 && txtAutocompleteAddress.Text.Length != 0 && !isChosen)
            {
                searchAutocomplete();
            }
            if (isChosen && tick == 2) isChosen = false;
        }

        private async void suggestionChosen(object sender, ItemClickEventArgs e)
        {
            isChosen = true;
            AutocompleteAddress item = (AutocompleteAddress)e.ClickedItem;
            txtAutocompleteAddress.Text = item.description;
            string reqUri = "https://maps.googleapis.com/maps/api/geocode/json?address=" + item.description + "&key=AIzaSyA9rHJZEGWe6rX4nAHTGXFxCubmw-F0BBw";
            string responseData = await httpObject.GetRequest(reqUri);
            JObject json = JObject.Parse(responseData);
            lat = json["results"][0]["geometry"]["location"]["lat"].ToString().Replace(",", ".");
            lng = json["results"][0]["geometry"]["location"]["lng"].ToString().Replace(",", ".");
            listAutoCompleteAddress.Clear();
        }

        private async void searchAutocomplete()
        {
            string input = txtAutocompleteAddress.Text;
            string reqUri = "https://maps.googleapis.com/maps/api/place/autocomplete/json?input=" + input + "&types=geocode&location=-7.252115,112.752849&radius=20000&language=id&components=country:id&strictbounds&key=AIzaSyA9rHJZEGWe6rX4nAHTGXFxCubmw-F0BBw";
            string responseData = await httpObject.GetRequest(reqUri);
            JObject json = JObject.Parse(responseData);
            if (json["status"].ToString() == "OK")
            {
                listAutoCompleteAddress.Clear();
                var token = JToken.Parse(responseData)["predictions"].ToList().Count;
                for (int i = 0; i < token; i++)
                {
                    string description = json["predictions"][i]["description"].ToString();
                    string placeId = json["predictions"][i]["place_id"].ToString();
                    listAutoCompleteAddress.Add(new AutocompleteAddress(description, placeId));
                }
                lvSuggestion.ItemsSource = listAutoCompleteAddress;
                lvSuggestion.IsItemClickEnabled = true;
            }
            else
            {
                if (txtAutocompleteAddress.Text.Length != 0)
                {
                    listAutoCompleteAddress.Clear();
                    listAutoCompleteAddress.Add(new AutocompleteAddress("Tidak ada hasil ditemukan", ""));
                    lvSuggestion.ItemsSource = listAutoCompleteAddress;
                    lvSuggestion.IsItemClickEnabled = false;
                }
            }
        }

        private void pageLoaded(object sender, RoutedEventArgs e)
        {
            txtNotelpUser.Text = userLogin.telpon_user;
            txtNamaUser.Text = userLogin.nama_user;
            if (userLogin.lokasi_aktif_user == null)
            {
                txtStatusLokasiAktif.Text = "(Belum diatur)";
                txtLabelLokasi.Text = "Anda belum mengatur lokasi aktif";
                btnEditLokasi.Content = "Atur";
                btnEditLokasi.Tag = "new";
                btnDisableLokasi.Visibility = Visibility.Collapsed;
            }
            else
            {
                lokasiUser = userLogin.lokasi_aktif_user;
                txtStatusLokasiAktif.Text = "(Sudah diatur)";
                btnEditLokasi.Content = "Ubah";
                btnEditLokasi.Tag = "update";
                txtLabelLokasi.Text = lokasiUser;
                btnDisableLokasi.Visibility = Visibility.Visible;
            }
            txtStatusAccount.Text = userLogin.status_user == 1 ? "Premium Account - Berlaku hingga " + DateTime.Parse(userLogin.premium_available_until).ToString("dd MMMM yyyy", new System.Globalization.CultureInfo("id-ID")) : "Free Account";
            btnSubscribe.Visibility = userLogin.status_user == 1 ? Visibility.Collapsed : Visibility.Visible;
        }

        private void showEditPanel(object sender, RoutedEventArgs e)
        {
            if ((string)(sender as Button).Tag == "new")
            {
                txtAutocompleteAddress.Text = "";
                isChosen = false;
            }
            else
            {
                txtAutocompleteAddress.Text = lokasiUser;
                isChosen = true;
            }
            btnEditLokasi.IsEnabled = false;
            btnDisableLokasi.IsEnabled = false;
            txtLabelLokasi.Visibility = Visibility.Collapsed;
            stackLokasi.Visibility = Visibility.Visible;
        }

        private void hideEditPanel(object sender, RoutedEventArgs e)
        {
            txtAutocompleteAddress.Text = "";
            txtLabelLokasi.Visibility = Visibility.Visible;
            stackLokasi.Visibility = Visibility.Collapsed;
            btnEditLokasi.IsEnabled = true;
            btnDisableLokasi.IsEnabled = true;
        }

        private void disableLokasi(object sender, RoutedEventArgs e)
        {
            btnDisableLokasi.Visibility = Visibility.Collapsed;
            btnEditLokasi.Content = "Atur";
            txtStatusLokasiAktif.Text = "(Belum diatur)";
            txtLabelLokasi.Text = "Anda belum mengatur lokasi aktif";
            btnEditLokasi.Tag = "new";
            lokasiUser = null;
            lat = null;
            lng = null;
        }

        private async void editLokasi(object sender, RoutedEventArgs e)
        {
            string responseData = await httpObject.GetRequest("settings/checkKecamatanAvailable?lat=" + lat + "&lng=" + lng);
            JObject json = JObject.Parse(responseData);
            if (json["status"].ToString() == "1")
            {
                lokasiUser = txtAutocompleteAddress.Text;
                txtLabelLokasi.Text = lokasiUser;
                btnEditLokasi.Tag = "update";
                txtStatusLokasiAktif.Text = "(Sudah diatur)";
                btnEditLokasi.Content = "Ubah";
                btnDisableLokasi.Visibility = Visibility.Visible;
                hideEditPanel(sender, e);
            }
            else
            {
                var message = new MessageDialog(json["message"].ToString());
                await message.ShowAsync();
            }

        }

        private void txtAutocompleteAddressTextChanged(object sender, TextChangedEventArgs e)
        {
            if (txtAutocompleteAddress.Text.Length == 0)
            {
                listAutoCompleteAddress.Clear();
            }
            if (!dispatcherTimer.IsEnabled)
            {
                dispatcherTimer.Start();
            }
            tick = 0;
        }


        private void goToConfirmationPage(object sender, RoutedEventArgs e)
        {
            UpdateProfileParams param = new UpdateProfileParams(userLogin.id_user, txtNamaUser.Text, lokasiUser, lat, lng);
            this.Frame.Navigate(typeof(ConfirmationProfileUpdatePage), param);
        }

        private void goToSubscriptionPage(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(SubscriptionPage));
        }

        private void changePassword(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(ChangePasswordPage));
        }
    }
}
