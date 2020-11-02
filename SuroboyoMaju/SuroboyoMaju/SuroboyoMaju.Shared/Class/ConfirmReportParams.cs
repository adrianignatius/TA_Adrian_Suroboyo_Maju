﻿using System;
using System.Collections.Generic;
using System.Text;

namespace SuroboyoMaju.Shared.Class
{
    class ConfirmReportParams
    {
        public string tag_laporan { get; set; }
        public string judul_laporan { get; set; }
        
        public string jenis_laporan { get; set; }

        public string deskripsi_laporan { get; set; }

        public string lat_laporan { get; set; }

        public string lng_laporan { get; set; }

        public string alamat_laporan { get; set; }

        public int id_kecamatan { get; set; } 

        public SettingKategori kategori_selected { get; set; }

        public int combo_box_selected_index { get; set; }

        public UploadedImage image_laporan { get; set; }

        public string tanggal_laporan { get; set; }
        
        public string waktu_laporan { get; set; }

        public ConfirmReportParams(string tag_laporan,string judul_laporan, string jenis_laporan, string deskripsi_laporan, string lat_laporan, string lng_laporan, string alamat_laporan, int id_kecamatan, SettingKategori kategori_selected, int combo_box_selected_index, UploadedImage image_laporan,string tanggal_laporan,string waktu_laporan)
        {
            this.tag_laporan = tag_laporan;
            this.judul_laporan = judul_laporan;
            this.jenis_laporan = jenis_laporan;
            this.deskripsi_laporan = deskripsi_laporan;
            this.lat_laporan = lat_laporan;
            this.lng_laporan = lng_laporan;
            this.alamat_laporan = alamat_laporan;
            this.id_kecamatan = id_kecamatan;
            this.kategori_selected = kategori_selected;
            this.combo_box_selected_index = combo_box_selected_index;
            this.image_laporan = image_laporan;
            this.tanggal_laporan = tanggal_laporan;
            this.waktu_laporan = waktu_laporan;
        }
    }
}
