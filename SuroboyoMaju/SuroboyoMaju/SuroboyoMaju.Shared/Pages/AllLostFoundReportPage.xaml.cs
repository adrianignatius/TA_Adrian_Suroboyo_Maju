﻿using Newtonsoft.Json;
using SuroboyoMaju.Shared.Class;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;


namespace SuroboyoMaju.Shared.Pages
{
    public sealed partial class AllLostFoundReportPage : Page
    {
        Session session;
        HttpObject httpObject;
        User userLogin;
        ObservableCollection<LaporanLostFound> listLaporanLostFound = new ObservableCollection<LaporanLostFound>();
        public AllLostFoundReportPage()
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

        private void goToDetailPage(object sender, ItemClickEventArgs e)
        {
            LaporanLostFound selected = (LaporanLostFound)e.ClickedItem;
            string jenis_laporan = selected.jenis_laporan == 0 ? "Penemuan " + selected.jenis_barang : "Kehilangan " + selected.jenis_barang;
            ReportDetailPageParams param = new ReportDetailPageParams(selected.id_user_pelapor, selected.nama_user_pelapor, selected.id_laporan, selected.alamat_laporan, selected.tanggal_laporan, selected.waktu_laporan, selected.judul_laporan, jenis_laporan, selected.deskripsi_barang, selected.lat_laporan, selected.lng_laporan, "lostfound", selected.thumbnail_gambar, selected.status_laporan, null);
            session.setReportDetailPageParams(param);
            this.Frame.Navigate(typeof(ReportDetailPage));
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

        private async void loadLaporanLostFound()
        {
            showLoading();
            string responseData = await httpObject.GetRequestWithAuthorization("laporan/getLaporanLostFound", session.getTokenAuthorization());
            listLaporanLostFound = JsonConvert.DeserializeObject<ObservableCollection<LaporanLostFound>>(responseData);
            if (listLaporanLostFound.Count == 0)
            {
                stackEmpty.Visibility = Visibility.Visible;
                svListView.Visibility = Visibility.Collapsed;
                stackLoading.Visibility = Visibility.Collapsed;
            }
            else
            {
                hideLoading();
                lvLaporanLostFound.ItemsSource = listLaporanLostFound;
            }
        }

        private void goToFilterPage(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(FilterPage));
        }

        private void refreshPage(object sender, RoutedEventArgs e)
        {
            btnRefresh.Visibility = Visibility.Collapsed;
            loadLaporanLostFound();
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
                string reqUri = "laporan/getLaporanLostFoundWithFilter?tanggal_awal=" + param.tanggal_awal + "&tanggal_akhir=" + param.tanggal_akhir + "&jenis_laporan=" + param.getArrayJenisLaporan() + "&id_barang=" + param.getArrayIdKategori() + "&id_kecamatan=" + param.id_kecamatan;
                string responseData = await httpObject.GetRequestWithAuthorization(reqUri, session.getTokenAuthorization());
                listLaporanLostFound = JsonConvert.DeserializeObject<ObservableCollection<LaporanLostFound>>(responseData);
                if (listLaporanLostFound.Count == 0)
                {
                    stackEmpty.Visibility = Visibility.Visible;
                    svListView.Visibility = Visibility.Collapsed;
                    stackLoading.Visibility = Visibility.Collapsed;
                    txtEmptyState.Text = "Tidak ada laporan yang sesuai dengan kriteria pencarian";
                }
                else
                {
                    hideLoading();
                    lvLaporanLostFound.ItemsSource = listLaporanLostFound;
                }
            }
            else
            {
                loadLaporanLostFound();
            }
        }
    }
}
