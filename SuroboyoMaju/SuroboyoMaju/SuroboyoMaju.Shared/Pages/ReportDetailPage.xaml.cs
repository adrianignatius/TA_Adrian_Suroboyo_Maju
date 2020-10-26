﻿using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SuroboyoMaju.Shared.Class;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Net.Http;
using Windows.UI;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Xamarin.Essentials;

namespace SuroboyoMaju.Shared.Pages
{

    public sealed partial class ReportDetailPage : Page
    {
        ReportDetailPageParams param;
        User userLogin;
        ObservableCollection<KomentarLaporan> listKomentar;
        Session session;
        HttpObject httpObject;
        public ReportDetailPage()
        {
            this.InitializeComponent();
            session = new Session();
            httpObject = new HttpObject();
            listKomentar = new ObservableCollection<KomentarLaporan>();
        }

        private async void checkKonfirmasiLaporan()
        {
            string responseData = await httpObject.GetRequestWithAuthorization("laporan/checkKonfirmasiLaporan?id_laporan=" + param.id_laporan + "&id_user=" + userLogin.id_user, session.getTokenAuthorization());
            JObject json = JObject.Parse(responseData);
            if (json["count"].ToString() == "0")
            {
                btnKonfirmasi.IsEnabled = true;
            }
            else
            {
                btnKonfirmasi.IsEnabled = false;
            }
        }

        private void pageLoaded(object sender, RoutedEventArgs e)
        {
            param = session.getReportDetailPageParams();
            userLogin = session.getUserLogin();
            string type = "";
            if (param.tag == "kriminalitas")
            {
                txtJumlahKonfirmasiLaporan.Text = param.jumlah_konfirmasi.ToString();
                checkKonfirmasiLaporan();
            }
            else
            {
                btnKonfirmasi.Visibility = Visibility.Collapsed;
                type = param.tag == "Penemuan barang" ? "0" : "1";
                stackKonfirmasiLaporan.Visibility = Visibility.Collapsed;
            }
            if ((userLogin.id_user == param.id_user_pelapor) || (userLogin.status_user == 2))
            {
                btnChatPage.Visibility = Visibility.Collapsed;
                btnKonfirmasi.Visibility = Visibility.Collapsed;
            }
            if (param.status_laporan == 0)
            {
                txtStatusLaporan.Text = "Belum diverifikasi";
                txtStatusLaporan.Foreground = new SolidColorBrush(Colors.Red);
                btnShare.Visibility = Visibility.Collapsed;
                stackComment.Visibility = Visibility.Collapsed;
            }
            else
            {
                txtStatusLaporan.Text = "Sudah diverifikasi";
                txtStatusLaporan.Foreground = new SolidColorBrush(Colors.Green);
            }
            imageLaporan.Source = new BitmapImage(new Uri(session.getUrlGambarLaporan() + param.thumbnail_gambar));
            txtNamaPengguna.Text = param.nama_user_pelapor;
            txtTanggalUpload.Text = param.tanggal_laporan + " Pukul " + param.waktu_laporan;
            txtDeskripsiLaporan.Text = param.deskripsi_laporan;
            txtJudulLaporan.Text = param.judul_laporan;
            txtAlamatLaporan.Text = param.alamat_laporan;
            txtJenisLaporan.Text = param.jenis_laporan;
            loadKomentarLaporan();
            webVieMapLokasi.Navigate(new Uri(session.getUrlWebView() + "location-map.php?lat=" + param.lat_laporan + "&lng=" + param.lng_laporan + "&type=" + type));
        }

        private async void konfirmasiLaporan(object sender, RoutedEventArgs e)
        {
            ContentDialog confirmDialog = new ContentDialog
            {
                Title = "Apakah anda yakin ingin melakukan konfirmasi?",
                Content = "Dengan melakukan konfirmasi maka anda membantu meningkatkan kebenaran informasi laporan ini ",
                PrimaryButtonText = "Konfirmasi",
                CloseButtonText = "Batal"
            };
            ContentDialogResult result = await confirmDialog.ShowAsync();
            if (result == ContentDialogResult.Primary)
            {
                var content = new FormUrlEncodedContent(new[]{
                    new KeyValuePair<string, string>("id_laporan", param.id_laporan),
                    new KeyValuePair<string, string>("id_user", userLogin.id_user.ToString())
                });
                string responseData = await httpObject.PostRequestWithUrlEncoded("laporan/konfirmasiLaporanKriminalitas", content);
                JObject json = JObject.Parse(responseData);
                var messageDialog = new MessageDialog(json["message"].ToString());
                await messageDialog.ShowAsync();
                btnKonfirmasi.IsEnabled = false;
                txtJumlahKonfirmasiLaporan.Text = (Convert.ToInt32(txtJumlahKonfirmasiLaporan.Text) + 1).ToString();
            }
        }

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            On_BackRequested();
        }

        private bool On_BackRequested()
        {
            if (this.Frame.CanGoBack)
            {
                this.Frame.GoBack();
                return true;
            }
            return false;
        }

        private async void openMap(object sender, RoutedEventArgs e)
        {
            var location = new Location(double.Parse(param.lat_laporan, CultureInfo.InvariantCulture), double.Parse(param.lng_laporan, CultureInfo.InvariantCulture));
            await Map.OpenAsync(location, new MapLaunchOptions
            {
                Name = param.alamat_laporan,
                NavigationMode = Xamarin.Essentials.NavigationMode.None
            });
        }


        public async void loadKomentarLaporan()
        {
            string responseData = await httpObject.GetRequestWithAuthorization("laporan/getKomentarLaporan/" + param.id_laporan, session.getTokenAuthorization());
            listKomentar = JsonConvert.DeserializeObject<ObservableCollection<KomentarLaporan>>(responseData);
            lvKomentarLaporan.ItemsSource = listKomentar;
        }

        private async void shareLaporan(object sender, RoutedEventArgs e)
        {
            DateTime dt = DateTime.Parse(param.tanggal_laporan);
            string tanggal_format = dt.ToString("dd MMMM yyyy", new System.Globalization.CultureInfo("id-ID"));
            string message = "Terdapat laporan tentang " + param.jenis_laporan + " yang terjadi pada tanggal " + tanggal_format;
            message = param.tag == "kriminalitas" ? message += " yang terjadi di " + param.alamat_laporan : message += ". Lokasi terakhir barang ada di " + param.alamat_laporan;
            await Share.RequestAsync(new ShareTextRequest
            {
                Text = message,
                Title = "Share informasi " + param.tag
            });
        }
        private async void goToChatPage(object sender, RoutedEventArgs e)
        {
            string responseData = await httpObject.GetRequestWithAuthorization("user/checkHeaderChat?id_user_1=" + userLogin.id_user + "&id_user_2=" + param.id_user_pelapor, session.getTokenAuthorization());
            JObject json = JObject.Parse(responseData);
            ChatPageParams chatParam = new ChatPageParams(Convert.ToInt32(json["id_chat"].ToString()), userLogin.id_user, param.id_user_pelapor, param.nama_user_pelapor);
            session.setChatPageParams(chatParam);
            this.Frame.Navigate(typeof(PersonalChatPage));
        }

        private async void sendComment(object sender, RoutedEventArgs e)
        {
            if (txtKomentar.Text.Length != 0)
            {
                string isi_komentar = txtKomentar.Text;
                var content = new FormUrlEncodedContent(new[]{
                    new KeyValuePair<string, string>("id_laporan", param.id_laporan),
                    new KeyValuePair<string, string>("isi_komentar", isi_komentar),
                    new KeyValuePair<string, string>("id_user_komentar", userLogin.id_user.ToString())
                });
                string responseData = await httpObject.PostRequestUrlEncodedWithAuthorization("user/insertKomentarLaporan", content, session.getTokenAuthorization());
                JObject json = JObject.Parse(responseData);
                var message = new MessageDialog(json["message"].ToString());
                await message.ShowAsync();
                txtKomentar.Text = "";
                loadKomentarLaporan();
            }
            else
            {
                var messageBox = new MessageDialog("Komentar yang dimasukkan tidak boleh kosong!");
                await messageBox.ShowAsync();
            }
        }
    }
}
