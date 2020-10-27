using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SuroboyoMaju.Shared.Class;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace SuroboyoMaju.Shared.Pages
{
    public sealed partial class AllCrimeReportPage : Page
    {
        Session session;
        HttpObject httpObject;
        ObservableCollection<LaporanKriminalitas> listLaporanKriminalitas = new ObservableCollection<LaporanKriminalitas>();
        int page = 0;
        int jumlah_laporan = 0;
        public AllCrimeReportPage()
        {
            this.InitializeComponent();
            session = new Session();
            httpObject = new HttpObject();
        }

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            session.setFilterState(0);
            this.Frame.GoBack();
        }


        private async void loadLaporanKriminalitas()
        {
            showLoading();
            string responseData = await httpObject.GetRequestWithAuthorization("laporan/getLaporanKriminalitas/" + page, session.getTokenAuthorization());
            listLaporanKriminalitas = JsonConvert.DeserializeObject<ObservableCollection<LaporanKriminalitas>>(responseData);
            if (listLaporanKriminalitas.Count == 0)
            {
                stackEmpty.Visibility = Visibility.Visible;
                svListView.Visibility = Visibility.Collapsed;
                stackLoading.Visibility = Visibility.Collapsed;
            }
            else
            {
                hideLoading();
                lvLaporanKriminalitas.ItemsSource = listLaporanKriminalitas;
            }
        }

        private void goToFilterPage(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(FilterPage));
        }

        private void showLoading()
        {
            stackLoading.Visibility = Visibility.Visible;
            svListView.Visibility = Visibility.Collapsed;
        }

        private void hideLoading()
        {
            stackLoading.Visibility = Visibility.Collapsed;
            svListView.Visibility = Visibility.Visible;
        }

        private void refreshPage(object sender, RoutedEventArgs e)
        {
            page = 0;
            setJumlahLaporan();
            btnRefresh.Visibility = Visibility.Collapsed;
            btnPrevPage.Visibility = Visibility.Collapsed;
            btnNextPage.Visibility = Visibility.Visible;
            stackEmpty.Visibility = Visibility.Collapsed;
            session.setFilterState(0);
            loadLaporanKriminalitas();
        }

        private void goToDetailPage(object sender, ItemClickEventArgs e)
        {
            LaporanKriminalitas selected = (LaporanKriminalitas)e.ClickedItem;
            ReportDetailPageParams param = new ReportDetailPageParams(selected.id_user_pelapor, selected.nama_user_pelapor, selected.id_laporan, selected.alamat_laporan, selected.tanggal_laporan, selected.waktu_laporan, selected.judul_laporan, selected.jenis_kejadian, selected.deskripsi_kejadian, selected.lat_laporan, selected.lng_laporan, "kriminalitas", selected.thumbnail_gambar, selected.status_laporan, selected.jumlah_konfirmasi);
            session.setPageState(page);
            session.setReportDetailPageParams(param);
            this.Frame.Navigate(typeof(ReportDetailPage));
        }

        private void nextPage(object sender,RoutedEventArgs e)
        {
            page++;
            int jumlah = (page+1) * 5;
            if (session.getFilterState() == 0)
            {
                loadLaporanKriminalitas();
            }
            else
            {
                loadLaporanKriminalitasWithFilter();
            }
            btnPrevPage.Visibility = Visibility.Visible;
            btnNextPage.Visibility = jumlah > jumlah_laporan ? Visibility.Collapsed : Visibility.Visible;
        }

        private void prevPage(object sender, RoutedEventArgs e)
        {
            page--;
            btnNextPage.Visibility = Visibility.Visible;
            if (session.getFilterState() == 0)
            {
                loadLaporanKriminalitas();
            }
            else
            {
                loadLaporanKriminalitasWithFilter();
            }
            btnPrevPage.Visibility = page == 0 ? Visibility.Collapsed : Visibility.Visible;
        }

        private async void loadLaporanKriminalitasWithFilter()
        {
            FilterParams param = session.getFilterParams();
            string reqUri = "laporan/getLaporanKriminalitasWithFilter/"+page+"?tanggal_awal=" + param.tanggal_awal + "&tanggal_akhir=" + param.tanggal_akhir + "&id_kejadian=" + param.getArrayIdKategori() + "&id_kecamatan=" + param.id_kecamatan;
            string responseData = await httpObject.GetRequestWithAuthorization(reqUri, session.getTokenAuthorization());
            listLaporanKriminalitas = JsonConvert.DeserializeObject<ObservableCollection<LaporanKriminalitas>>(responseData);
            if (listLaporanKriminalitas.Count == 0)
            {
                stackEmpty.Visibility = Visibility.Visible;
                svListView.Visibility = Visibility.Collapsed;
                stackLoading.Visibility = Visibility.Collapsed;
                txtEmptyState.Text = "Tidak ada laporan yang sesuai dengan kriteria pencarian";
            }
            else
            {
                hideLoading();
                lvLaporanKriminalitas.ItemsSource = listLaporanKriminalitas;
            }
        }

        private async void setJumlahLaporan()
        {
            string responseData = await httpObject.GetRequestWithAuthorization("laporan/getJumlahLaporanKriminalitas", session.getTokenAuthorization());
            JObject json = JObject.Parse(responseData);
            jumlah_laporan = Convert.ToInt32(json["count"].ToString());
            session.setJumlahLaporanState(jumlah_laporan);
        }

        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            var entry = this.Frame.BackStack.LastOrDefault();
            if (entry.SourcePageType == typeof(FilterPage))
            {
                showLoading();
                this.Frame.BackStack.RemoveAt(this.Frame.BackStackDepth - 1);
                this.Frame.BackStack.RemoveAt(this.Frame.BackStackDepth - 1);
                FilterParams param = session.getFilterParams();
                string reqUri = "laporan/getJumlahLaporanKriminalitasWithFilter?tanggal_awal=" + param.tanggal_awal + "&tanggal_akhir=" + param.tanggal_akhir + "&id_kejadian=" + param.getArrayIdKategori() + "&id_kecamatan=" + param.id_kecamatan;
                string responseData = await httpObject.GetRequestWithAuthorization(reqUri, session.getTokenAuthorization());
                JObject json = JObject.Parse(responseData);
                jumlah_laporan = Convert.ToInt32(json["count"].ToString());
                session.setJumlahLaporanState(jumlah_laporan);
                btnRefresh.Visibility = Visibility.Visible;
                loadLaporanKriminalitasWithFilter();
                btnNextPage.Visibility = jumlah_laporan < 6 ? Visibility.Collapsed : Visibility.Visible;
            }
            else if (entry.SourcePageType == typeof(ReportDetailPage))
            {
                this.Frame.BackStack.RemoveAt(this.Frame.BackStackDepth - 1);
                this.Frame.BackStack.RemoveAt(this.Frame.BackStackDepth - 1);
                int searchMode = session.getFilterState();
                page = session.getPageState();
                jumlah_laporan = session.getJumlahLaporanState();
                if (searchMode == 0){
                    loadLaporanKriminalitas();
                }else{
                    loadLaporanKriminalitasWithFilter();
                    btnRefresh.Visibility = Visibility.Visible;
                }   
                btnPrevPage.Visibility = page != 0 ? Visibility.Visible : Visibility.Collapsed;
                btnNextPage.Visibility = (page + 1) * 5 > jumlah_laporan ? Visibility.Collapsed : Visibility.Visible;
            }
            else
            {
                setJumlahLaporan();
                loadLaporanKriminalitas();
            }
        }
    }
}
