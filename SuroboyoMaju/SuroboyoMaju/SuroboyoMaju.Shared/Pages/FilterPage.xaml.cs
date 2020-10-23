﻿using Newtonsoft.Json;
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
using Windows.UI.Xaml.Navigation;

namespace SuroboyoMaju.Shared.Pages
{
    public sealed partial class FilterPage : Page
    {
        static int mode;
        ObservableCollection<Kecamatan> listKecamatan;
        List<Kecamatan> listKecamatanSelected;
        List<int> listIdBarangSelected;
        List<int> listIdKejadianSelected;
        List<int> listJenisLaporan;
        Session session;
        HttpObject httpObject;
        public FilterPage()
        {
            this.InitializeComponent();
            session = new Session();
            httpObject = new HttpObject();
            listKecamatan = new ObservableCollection<Kecamatan>();
            listIdBarangSelected = new List<int>();
            listJenisLaporan = new List<int>();
            listIdKejadianSelected = new List<int>();
            listKecamatanSelected = new List<Kecamatan>();
        }

        private void pageLoaded(object sender, RoutedEventArgs e)
        {
            DateTime now = DateTime.Now;
            DateTime before = now.AddMonths(-1);
            dtTanggalAwal.MaxYear = new DateTime(2023, 12, 31);
            dtTanggalAwal.MinYear = new DateTime(2020, 1, 31);
            dtTanggalAkhir.MaxYear = new DateTime(2023, 12, 31);
            dtTanggalAkhir.MinYear = new DateTime(2020, 1, 31);
            dtTanggalAwal.Date = before;
            dtTanggalAkhir.Date = now;
            rbAllKecamatan.IsChecked = true;
        }

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.GoBack();
        }

        private async void setFilter(object sender, RoutedEventArgs e)
        {
            if (validateInput() == true)
            {
                DateTime tanggal_awal = dtTanggalAwal.Date.DateTime;
                DateTime tanggal_akhir = dtTanggalAkhir.Date.DateTime;
                if (DateTime.Compare(tanggal_awal, tanggal_akhir) > 0)
                {
                    var message = new MessageDialog("Tanggal awal tidak boleh lebih besar dari tanggal akhir");
                    await message.ShowAsync();
                }
                else
                {

                    int id_kecamatan = rbSelectKecamatan.IsChecked == true ? Convert.ToInt32(cbKecamatan.SelectedValue.ToString()) : 0;
                    if (mode == 0)
                    {
                        FilterParams param = new FilterParams(tanggal_awal.ToString("yyyy-MM-dd"), tanggal_akhir.ToString("yyyy-MM-dd"), listJenisLaporan, listIdBarangSelected, id_kecamatan);
                        session.setFilterParams(param);
                        this.Frame.Navigate(typeof(AllLostFoundReportPage));
                    }
                    else
                    {
                        FilterParams param = new FilterParams(tanggal_awal.ToString("yyyy-MM-dd"), tanggal_akhir.ToString("yyyy-MM-dd"), null, listIdKejadianSelected, id_kecamatan);
                        session.setFilterParams(param);
                        this.Frame.Navigate(typeof(AllCrimeReportPage));
                    }
                }

            }
            else
            {
                var messageDialog = new MessageDialog("Harap isi semua kriteria yang dibutuhkan untuk pencarian");
                await messageDialog.ShowAsync();
            }
        }
        private bool validateInput()
        {
            if (dtTanggalAwal.SelectedDate == null || dtTanggalAkhir.SelectedDate == null)
            {
                return false;
            }
            else
            {
                if (mode == 0)
                {
                    if (listJenisLaporan.Count == 0 || listIdBarangSelected.Count == 0)
                    {
                        return false;
                    }
                    else
                    {
                        return true;
                    }
                }
                else
                {
                    if (listIdKejadianSelected.Count == 0)
                    {
                        return false;
                    }
                    else
                    {
                        return true;
                    }
                }

            }
        }


        private void jenisKejadianChecked(object sender, RoutedEventArgs e)
        {
            int id_kejadian = Convert.ToInt32((sender as CheckBox).Tag.ToString());
            listIdKejadianSelected.Add(id_kejadian);
        }

        private void jenisKejadianUnchecked(object sender, RoutedEventArgs e)
        {
            int id_kejadian = Convert.ToInt32((sender as CheckBox).Tag.ToString());
            listIdKejadianSelected.Remove(id_kejadian);
        }

        private void jenisLaporanChecked(object sender, RoutedEventArgs e)
        {
            int id_jenis_laporan = Convert.ToInt32((sender as CheckBox).Tag.ToString());
            listJenisLaporan.Add(id_jenis_laporan);
        }

        private void jenisLaporanUnchecked(object sender, RoutedEventArgs e)
        {
            int id_jenis_laporan = Convert.ToInt32((sender as CheckBox).Tag.ToString());
            listJenisLaporan.Remove(id_jenis_laporan);
        }

        private void jenisBarangChecked(object sender, RoutedEventArgs e)
        {
            int id_barang = Convert.ToInt32((sender as CheckBox).Tag.ToString());
            listIdBarangSelected.Add(id_barang);
        }

        private void jenisBarangUnchecked(object sender, RoutedEventArgs e)
        {
            int id_barang = Convert.ToInt32((sender as CheckBox).Tag.ToString());
            listIdBarangSelected.Remove(id_barang);
        }

        private void kecamatanUnchecked(object sender, RoutedEventArgs e)
        {
            int id_kecamatan = Convert.ToInt32((sender as CheckBox).Tag.ToString());
            Kecamatan selected = listKecamatan.Single(i => i.id_kecamatan == id_kecamatan);
            listKecamatanSelected.Remove(selected);
        }

        private void kecamatanChecked(object sender, RoutedEventArgs e)
        {
            int id_kecamatan = Convert.ToInt32((sender as CheckBox).Tag.ToString());
            Kecamatan selected = listKecamatan.Single(i => i.id_kecamatan == id_kecamatan);
            listKecamatanSelected.Add(selected);
        }

        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            stackLoading.Visibility = Visibility.Visible;
            stackFilter.Visibility = Visibility.Collapsed;
            var entry = this.Frame.BackStack.LastOrDefault();
            if (entry.SourcePageType == typeof(AllLostFoundReportPage))
            {
                stackBarang.Visibility = Visibility.Visible;
                stackJenisLaporan.Visibility = Visibility.Visible;
                stackKejadian.Visibility = Visibility.Collapsed;
                mode = 0;
            }
            else if (entry.SourcePageType == typeof(AllCrimeReportPage))
            {
                stackKejadian.Visibility = Visibility.Visible;
                stackBarang.Visibility = Visibility.Collapsed;
                stackJenisLaporan.Visibility = Visibility.Collapsed;
                mode = 1;
            }
            string responseData = await httpObject.GetRequest("settings/getKecamatan");
            listKecamatan = JsonConvert.DeserializeObject<ObservableCollection<Kecamatan>>(responseData);
            cbKecamatan.ItemsSource = listKecamatan;
            cbKecamatan.DisplayMemberPath = "nama_kecamatan";
            cbKecamatan.SelectedValuePath = "id_kecamatan";
            cbKecamatan.SelectedIndex = 0;
            stackLoading.Visibility = Visibility.Collapsed;
            stackFilter.Visibility = Visibility.Visible;
        }

        private void rbAllKecamatanChecked(object sender, RoutedEventArgs e)
        {
            cbKecamatan.Visibility = Visibility.Collapsed;
        }

        private void rbSelectKecamatanChecked(object sender, RoutedEventArgs e)
        {
            cbKecamatan.Visibility = Visibility.Visible;
        }
    }
}
