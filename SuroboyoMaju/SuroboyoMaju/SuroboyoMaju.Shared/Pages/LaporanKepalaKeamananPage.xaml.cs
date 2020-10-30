using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SuroboyoMaju.Shared.Class;
using System;
using System.Collections.ObjectModel;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace SuroboyoMaju.Shared.Pages
{
    public sealed partial class LaporanKepalaKeamananPage : Page
    {
        Session session;
        HttpObject httpObject;
        User userLogin;
        int page = 0;
        int jumlah_laporan = 0;
        ObservableCollection<LaporanKriminalitas> listLaporanKriminalitas;
        public LaporanKepalaKeamananPage()
        {
            this.InitializeComponent();
            session = new Session();
            httpObject = new HttpObject();
        }



        private void goToDetailPage(object sender, ItemClickEventArgs e)
        {
            LaporanKriminalitas selected = (LaporanKriminalitas)e.ClickedItem;
            ReportDetailPageParams param = new ReportDetailPageParams(selected.id_user_pelapor, selected.nama_user_pelapor, selected.id_laporan, selected.alamat_laporan, selected.tanggal_laporan, selected.waktu_laporan, selected.judul_laporan, selected.jenis_kejadian, selected.deskripsi_kejadian, selected.lat_laporan, selected.lng_laporan, "kriminalitas", selected.thumbnail_gambar, selected.status_laporan, selected.jumlah_konfirmasi);
            session.setReportDetailPageParams(param);
            this.Frame.Navigate(typeof(ReportDetailPage));
        }

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.GoBack();
        }

        private void nextPage(object sender, RoutedEventArgs e)
        {
            page++;
            int jumlah = (page + 1) * 5;
            loadLaporanKriminalitas();
            btnPrevPage.Visibility = Visibility.Visible;
            btnNextPage.Visibility = jumlah > jumlah_laporan ? Visibility.Collapsed : Visibility.Visible;
        }

        private void prevPage(object sender, RoutedEventArgs e)
        {
            page--;
            btnNextPage.Visibility = Visibility.Visible;
            loadLaporanKriminalitas();
            btnPrevPage.Visibility = page == 0 ? Visibility.Collapsed : Visibility.Visible;
        }

        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            userLogin = session.getUserLogin();
            txtJudulHalaman.Text = "Daftar laporan di area kecamatan " + userLogin.kecamatan_user;
            string responseData = await httpObject.GetRequestWithAuthorization("kepalaKeamanan/getJumlahLaporanKriminalitasKecamatan/"+userLogin.id_kecamatan_user, session.getTokenAuthorization());
            JObject json = JObject.Parse(responseData);
            jumlah_laporan = Convert.ToInt32(json["count"].ToString());
            session.setJumlahLaporanState(jumlah_laporan);
            btnNextPage.Visibility = jumlah_laporan < 6 ? Visibility.Collapsed : Visibility.Visible;
            loadLaporanKriminalitas();
        }

        private async void loadLaporanKriminalitas()
        {
            string responseData = await httpObject.GetRequestWithAuthorization("kepalaKeamanan/getLaporanKriminalitas/" + userLogin.id_kecamatan_user +"/"+page, session.getTokenAuthorization());
            listLaporanKriminalitas = JsonConvert.DeserializeObject<ObservableCollection<LaporanKriminalitas>>(responseData);
            if (listLaporanKriminalitas.Count == 0)
            {
                stackEmpty.Visibility = Visibility.Visible;
                svListView.Visibility = Visibility.Collapsed;
            }
            lvLaporanArea.ItemsSource = listLaporanKriminalitas;
        }
    }
}
