﻿using System;
using System.Collections.Generic;
using System.Text;

namespace SuroboyoMaju.Shared.Class
{
    class FilterParams
    {
        public string tanggal_awal { get; set; }

        public string tanggal_akhir { get; set; }

        public List<int> list_jenis_laporan { get; set; }

        public List<int> list_id_kategori { get; set; }

        public int id_kecamatan { get; set; }

        public FilterParams(string tanggal_awal, string tanggal_akhir, List<int> listJenisLaporan, List<int> list_id_kategori, int id_kecamatan)
        {
            this.tanggal_awal = tanggal_awal;
            this.tanggal_akhir = tanggal_akhir;
            this.list_jenis_laporan = listJenisLaporan;
            this.list_id_kategori = list_id_kategori;
            this.id_kecamatan = id_kecamatan;
        }

        public string getArrayJenisLaporan()
        {
            string s = "";
            for (int i = 0; i < list_jenis_laporan.Count; i++)
            {
                s += list_jenis_laporan[i].ToString() + ",";
            }
            return s.Remove(s.Length - 1);
        }

        public string getArrayIdKategori()
        {
            string s = "";
            for (int i = 0; i < list_id_kategori.Count; i++)
            {
                s += list_id_kategori[i].ToString() + ",";
            }
            return s.Remove(s.Length - 1);
        }
    }
}
