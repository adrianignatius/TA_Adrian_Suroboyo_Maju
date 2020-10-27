using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SuroboyoMaju.Shared.Class;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.InteropServices.WindowsRuntime;
using System.ServiceModel.Channels;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace SuroboyoMaju.Shared.Pages
{
    public sealed partial class AllCrimeReportPage : Page
    {
        Session session;
        HttpObject httpObject;
        User userLogin;
        ObservableCollection<LaporanKriminalitas> listLaporanKriminalitas = new ObservableCollection<LaporanKriminalitas>();
        int page = 0;
        int jumlah_laporan = 0;
        public AllCrimeReportPage()
        {
            this.InitializeComponent();
            session = new Session();
            httpObject = new HttpObject();
            userLogin = session.getUserLogin();
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
            jumlah_laporan = session.getJumlahLaporanState();
            btnRefresh.Visibility = Visibility.Collapsed;
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
            loadLaporanKriminalitas();
            btnPrevPage.Visibility = Visibility.Visible;
            if (jumlah > jumlah_laporan)
            {
                btnNextPage.Visibility = Visibility.Collapsed;
            }
            else
            {
                btnNextPage.Visibility = Visibility.Visible;
            }
            
        }

        private void prevPage(object sender, RoutedEventArgs e)
        {
            page--;
            btnNextPage.Visibility = Visibility.Visible;
            if (page == 0)
            {
                btnPrevPage.Visibility = Visibility.Collapsed;
            }
            else
            {
                btnPrevPage.Visibility = Visibility.Visible;
            }
            loadLaporanKriminalitas();
        }

        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            var entry = this.Frame.BackStack.LastOrDefault();
            if (entry.SourcePageType == typeof(FilterPage))
            {
                btnRefresh.Visibility = Visibility.Visible;
                showLoading();
                FilterParams param = session.getFilterParams();
                this.Frame.BackStack.RemoveAt(this.Frame.BackStackDepth - 1);
                this.Frame.BackStack.RemoveAt(this.Frame.BackStackDepth - 1);
                string reqUri = "laporan/getLaporanKriminalitasWithFilter?tanggal_awal=" + param.tanggal_awal + "&tanggal_akhir=" + param.tanggal_akhir + "&id_kejadian=" + param.getArrayIdKategori() + "&id_kecamatan=" + param.id_kecamatan;
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
            }else if (entry.SourcePageType == typeof(ReportDetailPage))
            {
                jumlah_laporan = session.getJumlahLaporanState();
                page = session.getPageState();
                loadLaporanKriminalitas();
                this.Frame.BackStack.RemoveAt(this.Frame.BackStackDepth - 1);
                this.Frame.BackStack.RemoveAt(this.Frame.BackStackDepth - 1);
                btnPrevPage.Visibility = page != 0 ? Visibility.Visible : Visibility.Collapsed;
                btnNextPage.Visibility = (page + 1) * 5 > jumlah_laporan ? Visibility.Collapsed : Visibility.Visible;
            }
            else
            {
                string responseData = await httpObject.GetRequestWithAuthorization("laporan/getJumlahLaporanKriminalitas", session.getTokenAuthorization());
                JObject json = JObject.Parse(responseData);
                jumlah_laporan = Convert.ToInt32(json["count"].ToString());
                session.setJumlahLaporanState(jumlah_laporan);
                loadLaporanKriminalitas();
            }
        }
    }
}
