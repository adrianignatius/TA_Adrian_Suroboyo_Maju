﻿using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SuroboyoMaju.Shared.Class;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Net.Http;
using System.Threading.Tasks;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Xamarin.Essentials;

namespace SuroboyoMaju.Shared.Pages
{
    public sealed partial class HomePage : Page
    {
        Session session;
        User userLogin;
        HttpObject httpObject;
        ObservableCollection<LaporanLostFound> listLaporanLostFound;
        ObservableCollection<LaporanKriminalitas> listLaporanKriminalitas;
        ObservableCollection<User> listEmergencyContact;
        public HomePage()
        {
            this.InitializeComponent();
            session = new Session();
            httpObject = new HttpObject();
            
        }
        public async void pageLoaded(object sender, RoutedEventArgs e)
        {
            userLogin = session.getUserLogin();
            txtNamaUser.Text = "Selamat Datang, " + userLogin.nama_user + "!";
            if (userLogin.status_user == 1)
            {
                txtStatusUser.Text = "Premium Account";
            }
            else
            {
                txtStatusUser.Text = "Free Account";
            }
            string responseData = await httpObject.GetRequestWithAuthorization("laporan/getHeadlineLaporanKriminalitas", session.getTokenAuthorization());
            listLaporanKriminalitas = JsonConvert.DeserializeObject<ObservableCollection<LaporanKriminalitas>>(responseData);
            responseData = await httpObject.GetRequestWithAuthorization("laporan/getHeadlineLaporanLostFound", session.getTokenAuthorization());
            listLaporanLostFound = JsonConvert.DeserializeObject<ObservableCollection<LaporanLostFound>>(responseData);
#if __ANDROID__
            lvHeadline.ItemsSource = listLaporanLostFound;
            btnSelectionLaporanKriminalitas.IsEnabled = true;
            btnSelectionLaporanLostFound.IsEnabled = false;
            lvHeadline.Tag = "lvLostfound";
            if (userLogin.status_user == 1)
            {
                btnEmergency.Visibility = Visibility.Visible;
            }
            else
            {
                btnEmergency.Visibility = Visibility.Collapsed;
            }
#elif NETFX_CORE
            lvLaporanKriminalitas.ItemsSource = listLaporanKriminalitas;
            lvLaporanLostFound.ItemsSource = listLaporanLostFound;
#endif 
        }

#if __ANDROID__
        private void changeSource(object sender,RoutedEventArgs e)
        {
            string tag = (sender as Button).Tag.ToString();
            if (tag == "1")
            {
                lvHeadline.ItemsSource = listLaporanLostFound;
                btnSelectionLaporanKriminalitas.IsEnabled = true;
                btnSelectionLaporanLostFound.IsEnabled = false;
                lvHeadline.Tag = "lvLostfound";
                txtTagLine.Tag="lostfound";
                txtHeadlineTitle.Text = "Laporan Lost & Found Terkini";
            }
            else
            {
                lvHeadline.ItemsSource = listLaporanKriminalitas;
                btnSelectionLaporanKriminalitas.IsEnabled = false;
                btnSelectionLaporanLostFound.IsEnabled = true;
                lvHeadline.Tag = "lvKriminalitas";
                txtTagLine.Tag="kriminalitas";
                txtHeadlineTitle.Text = "Laporan Kriminalitas";
            }
        }

        private async void emergencyAction(object sender, RoutedEventArgs e)
        {
            ContentDialog confirmDialog = new ContentDialog
                {
                    Title = "Apakah anda yakin ingin mengirim pesan darurat?",
                    Content = "Pesan ini akan disampaikan kepada seluruh kontak anda",
                    PrimaryButtonText = "Ya",
                    CloseButtonText = "Tidak"
                };
            ContentDialogResult result = await confirmDialog.ShowAsync();
            if(result==ContentDialogResult.Primary){
                sendNotification();
            } 
        }

        private async void sendNotification()
        {
            string responseData=await httpObject.GetRequestWithAuthorization("user/getEmergencyContact/"+userLogin.id_user,session.getTokenAuthorization());
            listEmergencyContact = JsonConvert.DeserializeObject<ObservableCollection<User>>(responseData);
            if(listEmergencyContact.Count<1){
                var message = new MessageDialog("Tidak ada kontak darurat yang terdaftar");
                await message.ShowAsync();
            }
                
            else
            {
                string address = await getUserAddress();
                string heading=userLogin.nama_user+" sedang dalam keadaan darurat!";
                string messageContent="Salah satu kontakmu, "+userLogin.nama_user+" sedang dalam keadaan darurat. Lokasi terakhirnya ada di "+address;
                foreach (User user in listEmergencyContact)
                {
                    responseData = await httpObject.GetRequestWithAuthorization("user/checkHeaderChat?id_user_1=" + userLogin.id_user + "&id_user_2=" + user.id_user, session.getTokenAuthorization());
                    JObject json = JObject.Parse(responseData); 
                    sendEmergencyChat(user, address, json["id_chat"].ToString());
                    var content = new FormUrlEncodedContent(new[]
                    {
                        new KeyValuePair<string, string>("number", user.telpon_user),
                        new KeyValuePair<string, string>("heading", heading),
                        new KeyValuePair<string, string>("content", messageContent),
                        new KeyValuePair<string, string>("id_chat", json["id_chat"].ToString()),
                        new KeyValuePair<string, string>("id_user_pengirim", userLogin.id_user.ToString()),
                        new KeyValuePair<string, string>("id_user_penerima", user.id_user.ToString()),
                        new KeyValuePair<string, string>("nama_display", user.nama_user)
                    });
                    await httpObject.PostRequestUrlEncodedWithAuthorization("user/sendEmergencyNotification", content,session.getTokenAuthorization());
                }
                await new MessageDialog("Pesan darurat telah dikirimkan ke semua kontak darurat anda").ShowAsync();
            }
        }

            private async void sendEmergencyChat(User u, string address,string id_chat)
            {
                string message = "Saya sedang dalam keadaan darurat! Lokasi terakhir saya di " + address;
                string responseData = await httpObject.GetRequestWithAuthorization("user/checkHeaderChat?id_user_1=" + userLogin.id_user + "&id_user_2=" + u.id_user, session.getTokenAuthorization());
                JObject json = JObject.Parse(responseData);
                var content = new FormUrlEncodedContent(new[]{
                    new KeyValuePair<string, string>("id_chat", id_chat),
                    new KeyValuePair<string, string>("id_user_pengirim", userLogin.id_user.ToString()),
                    new KeyValuePair<string, string>("id_user_penerima", u.id_user.ToString()),
                    new KeyValuePair<string, string>("isi_chat", message),
                });
                responseData = await httpObject.PostRequestUrlEncodedWithAuthorization("user/insertDetailChat", content, session.getTokenAuthorization());
        }
#endif

        private async Task<string> getUserAddress()
        {
            var location = await Geolocation.GetLastKnownLocationAsync();
            if (location == null)
            {
                location = await Geolocation.GetLocationAsync(new GeolocationRequest
                {
                    DesiredAccuracy = GeolocationAccuracy.Best,
                    Timeout = TimeSpan.FromSeconds(30)
                });
            }
            string lat = location.Latitude.ToString().Replace(",", ".");
            string lng = location.Longitude.ToString().Replace(",", ".");
            string latlng = lat + "," + lng;
            string reqUri = "https://maps.googleapis.com/maps/api/geocode/json?latlng=" + latlng + "&key=AIzaSyA9rHJZEGWe6rX4nAHTGXFxCubmw-F0BBw";
            string responseData = await httpObject.GetRequest(reqUri);
            JObject json = JObject.Parse(responseData);
            string address = json["results"][0]["formatted_address"].ToString();
            return address;
        }

        public void goToDetailPage(object sender, ItemClickEventArgs e)
        {
            string tag = (sender as ListView).Tag.ToString();
            if (tag == "lvKriminalitas")
            {
                LaporanKriminalitas selected = (LaporanKriminalitas)e.ClickedItem;
                ReportDetailPageParams param = new ReportDetailPageParams(selected.id_user_pelapor, selected.nama_user_pelapor, selected.id_laporan, selected.alamat_laporan, selected.tanggal_laporan, selected.waktu_laporan, selected.judul_laporan, selected.jenis_kejadian, selected.deskripsi_kejadian, selected.lat_laporan, selected.lng_laporan, "kriminalitas", selected.thumbnail_gambar, selected.status_laporan, selected.jumlah_konfirmasi);
                session.setReportDetailPageParams(param);
            }
            else if (tag == "lvLostfound")
            {
                LaporanLostFound selected = (LaporanLostFound)e.ClickedItem;
                string jenis_laporan = selected.jenis_laporan == 0 ? "Penemuan " + selected.jenis_barang : "Kehilangan " + selected.jenis_barang;
                ReportDetailPageParams param = new ReportDetailPageParams(selected.id_user_pelapor, selected.nama_user_pelapor, selected.id_laporan, selected.alamat_laporan, selected.tanggal_laporan, selected.waktu_laporan, selected.judul_laporan, jenis_laporan, selected.deskripsi_barang, selected.lat_laporan, selected.lng_laporan, "lostfound", selected.thumbnail_gambar, selected.status_laporan, null);
                session.setReportDetailPageParams(param);
            }
            HomeNavigationPage page = session.getHomeNavigationPageInstance();
            page.Frame.Navigate(typeof(ReportDetailPage));
        }
        private void goToAllReportPage(object sender, RoutedEventArgs e)
        {
            string tag = (sender as TextBlock).Tag.ToString();
            if (tag == "kriminalitas")
            {
                if (userLogin.status_user == 2)
                {
                    session.getHomePageKepalaKeamananInstance().Frame.Navigate(typeof(AllCrimeReportPage));
                }
                else
                {
                    session.getHomeNavigationPageInstance().Frame.Navigate(typeof(AllCrimeReportPage));
                }
            }
            else
            {
                if (userLogin.status_user == 2)
                {
                    session.getHomePageKepalaKeamananInstance().Frame.Navigate(typeof(AllCrimeReportPage));
                }
                else
                {
                    session.getHomeNavigationPageInstance().Frame.Navigate(typeof(AllCrimeReportPage));
                }
            }
        }
    }
}
