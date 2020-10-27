﻿using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
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


namespace SuroboyoMaju.Shared.Pages
{
    public sealed partial class HistoryPage : Page
    {
        Session session;
        User userLogin;
        HttpObject httpObject;
        ObservableCollection<LaporanLostFound> listHistoryLaporanLostFound;
        ObservableCollection<LaporanKriminalitas> listHistoryLaporanKriminalitas;
        public HistoryPage()
        {
            this.InitializeComponent();
            session = new Session();
            userLogin = session.getUserLogin();
            httpObject = new HttpObject();
            listHistoryLaporanKriminalitas = new ObservableCollection<LaporanKriminalitas>();
            listHistoryLaporanLostFound = new ObservableCollection<LaporanLostFound>();
        }

        private void pageLoaded(object sender, RoutedEventArgs e)
        {
            setListViewLostFound();
        }

        public void goToDetailPage(object sender, ItemClickEventArgs e)
        {
            string tag = (sender as ListView).Tag.ToString();
            if (tag == "lvKriminalitas")
            {
                LaporanKriminalitas selected = (LaporanKriminalitas)e.ClickedItem;
                ReportDetailPageParams param = new ReportDetailPageParams(userLogin.id_user, userLogin.nama_user, selected.id_laporan, selected.alamat_laporan, selected.tanggal_laporan, selected.waktu_laporan, selected.judul_laporan, selected.jenis_kejadian, selected.deskripsi_kejadian, selected.lat_laporan, selected.lng_laporan, "kriminalitas", selected.thumbnail_gambar, selected.status_laporan, selected.jumlah_konfirmasi);
                session.setReportDetailPageParams(param); ;
            }
            else if (tag == "lvLostFound")
            {
                LaporanLostFound selected = (LaporanLostFound)e.ClickedItem;
                string jenis_laporan = selected.jenis_laporan == 0 ? "Penemuan " + selected.jenis_barang : "Kehilangan " + selected.jenis_barang;
                ReportDetailPageParams param = new ReportDetailPageParams(userLogin.id_user, userLogin.nama_user, selected.id_laporan, selected.alamat_laporan, selected.tanggal_laporan, selected.waktu_laporan, selected.judul_laporan, jenis_laporan, selected.deskripsi_barang, selected.lat_laporan, selected.lng_laporan, "lostfound", selected.thumbnail_gambar, selected.status_laporan, null);
                session.setReportDetailPageParams(param);
            }
            this.Frame.Navigate(typeof(ReportDetailPage));
        }

        private async void cancelLaporan(object sender, RoutedEventArgs e)
        {
            ContentDialog confirmDialog = new ContentDialog
            {
                Title = "Apakah anda yakin ingin membatalkan laporan ini?",
                Content = "Laporan ini akan dibatalkan dan tidak akan terlihat oleh pengguna lainnya",
                PrimaryButtonText = "Ya",
                CloseButtonText = "Tidak"
            };
            ContentDialogResult result = await confirmDialog.ShowAsync();
            if (result == ContentDialogResult.Primary)
            {
                string id_laporan = (sender as Button).Tag.ToString();
                string jenis_laporan = id_laporan.Substring(0, 1) == "L" ? "0" : "1";
                string responseData = await httpObject.PutRequest("laporan/cancelLaporan/" + jenis_laporan + "/" + id_laporan, null, session.getTokenAuthorization());
                JObject json = JObject.Parse(responseData);
                await new MessageDialog(json["message"].ToString()).ShowAsync();
                if (jenis_laporan == "0")
                {
                    loadLaporanLostFound();
                }
                else
                {
                    loadLaporanKriminalitas();
                }
            }
        }

        private void setListViewLostFound()
        {
            btnSelectionLaporanLostFound.IsEnabled = false;
            btnSelectionLaporanKriminalitas.IsEnabled = true;
            loadLaporanLostFound();
            lvHistory.Tag = "lvLostFound";
        }

        private void setListViewKriminalitas()
        {
            btnSelectionLaporanKriminalitas.IsEnabled = false;
            btnSelectionLaporanLostFound.IsEnabled = true;
            loadLaporanKriminalitas();
            lvHistory.Tag = "lvKriminalitas";
        }

        private async void loadLaporanLostFound()
        {
            string responseData = await httpObject.GetRequestWithAuthorization("user/getHistoryLaporanLostFound/" + userLogin.id_user, session.getTokenAuthorization());
            listHistoryLaporanLostFound = JsonConvert.DeserializeObject<ObservableCollection<LaporanLostFound>>(responseData);
            if (listHistoryLaporanLostFound.Count == 0)
            {
                stackEmpty.Visibility = Visibility.Visible;
                lvHistory.Visibility = Visibility.Collapsed;
            }
            lvHistory.ItemsSource = listHistoryLaporanLostFound;
        }

        private async void loadLaporanKriminalitas()
        {
            string responseData = await httpObject.GetRequestWithAuthorization("user/getHistoryLaporanKriminalitas/" + userLogin.id_user, session.getTokenAuthorization());
            listHistoryLaporanKriminalitas = JsonConvert.DeserializeObject<ObservableCollection<LaporanKriminalitas>>(responseData);
            if (listHistoryLaporanLostFound.Count == 0)
            {
                stackEmpty.Visibility = Visibility.Visible;
                lvHistory.Visibility = Visibility.Collapsed;
            }
            lvHistory.ItemsSource = listHistoryLaporanKriminalitas;
        }

        private void changeSource(object sender, RoutedEventArgs e)
        {
            string tag = (sender as Button).Tag.ToString();
            if (tag == "1")
            {
                setListViewLostFound();
            }
            else
            {
                setListViewKriminalitas();
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

    }
}
