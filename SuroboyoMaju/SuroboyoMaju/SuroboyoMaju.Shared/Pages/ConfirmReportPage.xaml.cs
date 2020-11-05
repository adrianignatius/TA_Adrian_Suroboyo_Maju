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
using Windows.UI.Xaml.Media.Imaging;

namespace SuroboyoMaju.Shared.Pages
{
    public sealed partial class ConfirmReportPage : Page
    {
        Session session;
        ConfirmReportParams param;
        User userLogin;
        HttpObject httpObject;
        string url = "ms-appx:///Assets/icon/";
        string tanggal, waktu;
        public ConfirmReportPage()
        {
            this.InitializeComponent();
            session = new Session();
            httpObject = new HttpObject();
        }

        private void pageLoaded(object sender, RoutedEventArgs e)
        {
            this.Frame.BackStack.RemoveAt(this.Frame.BackStack.Count - 1);
            userLogin = session.getUserLogin();
            param = session.getConfirmReportParams();
            imageIcon.Source = new BitmapImage(new Uri(url + param.kategori_selected.file_gambar_kategori));
            txtJenisLaporan.Text = param.kategori_selected.nama_kategori;
            tanggal = DateTime.Now.ToString("dd MMMM yyyy", new System.Globalization.CultureInfo("id-ID"));
            waktu = DateTime.Now.ToString("HH:mm:ss");
            txtTanggalLaporan.Text = tanggal+" Pukul "+waktu;
            txtJudulLaporan.Text = param.judul_laporan;
            txtDeskripsiLaporan.Text = param.deskripsi_laporan;
            txtLokasiLaporan.Text = param.alamat_laporan;
            if (param.tag_laporan == "lostfound")
            {
                if (param.jenis_laporan == "0")
                {
                    txtHeaderDetailLaporan.Text = "Konfirmasi Laporan Penemuan Barang";
                    txtHeaderLokasi.Text = "Lokasi penemuan barang";
                }
                else
                {
                    txtHeaderDetailLaporan.Text = "Konfirmasi Laporan Kehilangan Barang";
                    txtHeaderLokasi.Text = "Lokasi kehilangan barang";
                }
            }
            else
            {
                txtHeaderDetailLaporan.Text = "Konfirmasi Laporan Kejadian";
                txtHeaderLokasi.Text = "Lokasi kejadian";
                txtHeaderJenisLaporan.Text = "Jenis kejadian yang dilaporkan";
                txtHeaderDeskripsiLaporan.Text = "Deskripsi Kejadian";
            }
        }

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            if (param.tag_laporan == "kriminalitas")
            {
                this.Frame.Navigate(typeof(MakeCrimeReportPage));
            }
            else
            {
                this.Frame.Navigate(typeof(MakeLostFoundReportPage));
            }
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

        private async void konfirmasi_laporan(object sender, RoutedEventArgs e)
        {
            MultipartFormDataContent form = new MultipartFormDataContent();
            form.Add(new StringContent(param.judul_laporan), "judul_laporan");
            form.Add(new StringContent(DateTime.Now.ToString("yyyy-MM-dd")), "tanggal_laporan");
            form.Add(new StringContent(DateTime.Now.ToString("HH:mm:ss")), "waktu_laporan");
            form.Add(new StringContent(param.alamat_laporan), "alamat_laporan");
            form.Add(new StringContent(param.lat_laporan), "lat_laporan");
            form.Add(new StringContent(param.lng_laporan), "lng_laporan");
            form.Add(new StringContent(userLogin.id_user.ToString()), "id_user_pelapor");
            form.Add(new StringContent(param.id_kecamatan.ToString()), "id_kecamatan");
            var responseData = "";
            if (param.tag_laporan == "kriminalitas")
            {
                form.Add(new StringContent(param.kategori_selected.id_kategori.ToString()), "id_kategori_kejadian");
                form.Add(new StringContent(param.deskripsi_laporan), "deskripsi_kejadian");
                if (param.image_laporan != null)
                {
                    form.Add(new StreamContent(new MemoryStream(param.image_laporan.image)), "image", "image.jpg");
                }
                responseData = await httpObject.PostRequestWithMultipartFormData("laporan/insertLaporanKriminalitas", form, session.getTokenAuthorization());
            }
            else
            {
                form.Add(new StringContent(param.kategori_selected.id_kategori.ToString()), "id_kategori_barang");
                form.Add(new StringContent(param.jenis_laporan.ToString()), "jenis_laporan");
                form.Add(new StringContent(param.deskripsi_laporan), "deskripsi_barang");
                form.Add(new StreamContent(new MemoryStream(param.image_laporan.image)), "image", "image.jpg");
                responseData = await httpObject.PostRequestWithMultipartFormData("laporan/insertLaporanLostFound", form, session.getTokenAuthorization());
            }
            JObject json = JObject.Parse(responseData);
            var message = new MessageDialog(json["message"].ToString());
            await message.ShowAsync();
            if (json["status"].ToString() == "1")
            {
                this.Frame.Navigate(typeof(HomeNavigationPage));
            }
        }
    }
}
